using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Options;
using System.Reflection;
using System.Threading.Tasks;
using System.Runtime.Loader;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PlatoCore.FileSystem.Abstractions;
using PlatoCore.Models.Modules;
using PlatoCore.Modules.Abstractions;
using PlatoCore.Modules.Models;

namespace PlatoCore.Modules
{
    public class ModuleManager : IModuleManager
    {

        #region "Private Variables"

        private readonly IModuleLocator _moduleLocator;
        private readonly IModuleLoader _moduleLoader;
        private readonly IPlatoFileSystem _fileSystem;
        private readonly ILogger<ModuleManager> _logger;

        private readonly string _virtualPathToModules;
        private readonly string _contentRootPath;

        private IEnumerable<IModuleDescriptor> _moduleDescriptors;

        private static List<IModuleEntry> _moduleEntries;
        private static IDictionary<string, Assembly> _loadedAssemblies;

        #endregion

        #region "Public ReadOnly Propertoes"

        public IEnumerable<IModuleEntry> AvailableModules => _moduleEntries;

        public IEnumerable<Assembly> AllAvailableAssemblies => _loadedAssemblies.Values;

        #endregion

        #region "Constructor"

        public ModuleManager(
            IHostEnvironment hostingEnvironment,
            IOptions<ModuleOptions> moduleOptions,
            ILogger<ModuleManager> logger,
            IPlatoFileSystem fileSystem,
            IModuleLocator moduleLocator,
            IModuleLoader moduleLoader)
        {
            _virtualPathToModules = moduleOptions.Value.VirtualPathToModulesFolder;
            _contentRootPath = hostingEnvironment.ContentRootPath;
            _moduleLocator = moduleLocator;
            _moduleLoader = moduleLoader;
            _fileSystem = fileSystem;
            _logger = logger;
        }

        #endregion

        #region "Implementation"

        public async Task<IEnumerable<IModuleEntry>> LoadModulesAsync()
        {
            await InitializeModules();
            return _moduleEntries;
        }

        public async Task<IEnumerable<Assembly>> LoadModuleAssembliesAsync()
        {
            await InitializeModules();
            return _loadedAssemblies.Values;
        }

        public async Task<IEnumerable<Assembly>> LoadModuleAssembliesAsync(string[] moduleIds)
        {

            await InitializeModules();
     
            // Get descriptors for supplied moduleIds
            var descriptors = _moduleEntries
                .Where(e => moduleIds.Any(moduleId => moduleId.Equals(e.Descriptor.Id, StringComparison.OrdinalIgnoreCase)))
                .Select(d => d.Descriptor)
                .ToList();
          
            // Build all dependencies
            var loadedAssemblies = new List<Assembly>();
            foreach (var descriptor in descriptors)
            {
                var assemblies = await _moduleLoader.LoadModuleAsync(descriptor);
                var assembly = assemblies.FirstOrDefault(a => a.FullName == descriptor.Id);
                _moduleEntries.Add(new ModuleEntry()
                {
                    Descriptor = descriptor,
                    Assembly = assembly,
                    Assemblies = assemblies
                });
                loadedAssemblies.AddRange(assemblies);

            }

            return loadedAssemblies;

        }
        
        public async Task<IEnumerable<IModuleEntry>> LoadModulesAsync(string[] moduleIds)
        {
            await InitializeModules();
            
            var loadedModules = _moduleEntries
                .Where(m => moduleIds.Contains(m.Descriptor.Id)).ToList();

            return loadedModules;
        }

        #endregion

        #region "Private Methods"

        async Task InitializeModules()
        {
            if (_moduleEntries == null)
            {
                _moduleEntries = new List<IModuleEntry>();
                _loadedAssemblies = new Dictionary<string, Assembly>();
                await LocateModuleDescriptors();
                await LoadModulesInternalAsync();
            }
        }
        
        async Task LocateModuleDescriptors()
        {
            _moduleDescriptors = await _moduleLocator.LocateModulesAsync(
                new string[] { _contentRootPath + "\\" + _virtualPathToModules },
                "Module", "module.txt", false);
        }

        async Task LoadModulesInternalAsync()
        {

            if (_moduleDescriptors == null)
                throw new NullReferenceException(nameof(_moduleDescriptors));

            foreach (var descriptor in _moduleDescriptors)
            {

                // Load all assemblies within descriptors bin folder
                var assemblies = await _moduleLoader.LoadModuleAsync(descriptor);

                // Get assembly with name matching descriptor Id (i.e. the modules primary assembly)
                var primaryAssembly = assemblies.FirstOrDefault(a =>
                    Path.GetFileNameWithoutExtension(a.ManifestModule.Name) == descriptor.Id);
                           
                // The assembly may have already been loaded by another module
                // For example if a module references the modules assembly we are trying to load
                // LoadModuleAsync will only ever load the assembly once
                // In this case add already loaded assemblies for the module
                if (assemblies.Count == 0)
                {                    
                    assemblies = _loadedAssemblies
                        .Where(m => m.Key == descriptor.Id)
                        .Select(a => a.Value)
                        .ToList();
                }

                // If the module assembly is not found by LoadModuleAsync
                // It could be the module is precompiled and as such added
                // to the application assemblies via DependencyContext
                // within the ModularFeatureApplicationPart provider
                // LoadModuleAsync
                if (primaryAssembly == null)
                {
                    try
                    {
                        primaryAssembly = Assembly.Load(new AssemblyName(descriptor.Id));
                    }
                    catch 
                    {
                        // ignored
                    }
                    
                }
                
                // Attempt to get modules assembly again from any
                // previously loaded dependencies
                if (primaryAssembly == null)
                {
                    primaryAssembly = assemblies.FirstOrDefault(a =>
                        Path.GetFileNameWithoutExtension(a.ManifestModule.Name) == descriptor.Id);
                }
                
                // We need a modules primary assembly. 
                if (primaryAssembly == null)
                {
                    // Log load failure
                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogInformation($"Could not locate {descriptor.Id}.dll for module {descriptor.Id}. Please ensure the DLL exists within the modules /bin folder. If the modules DLL exists please ensure the folder containing your '{descriptor.Id}' module has the same name as your modules primary assembly name minus the .DLL extension. You can either rename the '{descriptor.Id}' modules primary assembly to '{descriptor.Id}.dll' or change the module folder name to match your existing assembly name minus the .DLL extension. For example if your modules assembly is named 'MyCustomModule.Data.dll' your module folder name should be named 'MyCustomModule.Data' so the full path would be 'Modules/MyCustomModule.Data'");
                    }
                }
                
                // Add the described module entry to our local list
                _moduleEntries.Add(new ModuleEntry()
                {
                    Descriptor = descriptor,
                    Assembly = primaryAssembly,
                    Assemblies = assemblies
                });

                // Add all located assemblies to local for query next time round
                foreach (var assembly in assemblies)
                {
                    var assemblyName = Path.GetFileNameWithoutExtension(assembly.ManifestModule.Name);
                    if (!_loadedAssemblies.ContainsKey(assemblyName))
                    {
                        _loadedAssemblies.Add(assemblyName, assembly);
                    }
                }

            }

        }

        #endregion

    }

}
