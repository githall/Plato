using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Plato.Internal.Modules.Abstractions;

namespace Plato.Internal.Layout.ViewFeatures
{

    public class ModularFeatureApplicationPart :
        ApplicationPart,
        IApplicationPartTypeProvider,
        ICompilationReferencesProvider
    {

        private static IEnumerable<string> _referencePaths;
        private static object _synLock = new object();

        private readonly IModuleManager _moduleManager;
        private readonly ITypedModuleProvider _typedModuleProvider;

        public ModularFeatureApplicationPart(IServiceProvider services)
        {
            _moduleManager = services.GetRequiredService<IModuleManager>();
            _typedModuleProvider = services.GetRequiredService<ITypedModuleProvider>();
        }

        public override string Name => nameof(ModularFeatureApplicationPart);

        /// <inheritdoc />
        public IEnumerable<TypeInfo> Types => _typedModuleProvider.GetTypesAsync()
                    .GetAwaiter()
                    .GetResult();

        /// <inheritdoc />
        public IEnumerable<string> GetReferencePaths()
        {
            if (_referencePaths != null)
            {
                return _referencePaths;
            }

            lock (_synLock)
            {

                if (_referencePaths != null)
                {
                    return _referencePaths;
                }

                // Build a list of all referenced assembly paths
                var referencePaths = new List<string>();

                // Add reference paths for all compiled libraries within our dependency context
                referencePaths.AddRange(DependencyContext.Default.CompileLibraries
                    .SelectMany(library => library.ResolveReferencePaths()));

                // Add reference paths for all available modules
                var assemblies = _moduleManager.LoadModuleAssembliesAsync().Result;
                if (assemblies != null)
                {
                    referencePaths.AddRange(assemblies
                        .Where(library => !library.IsDynamic && !string.IsNullOrWhiteSpace(library.Location))
                        .Select(library => library.Location));
                }

                _referencePaths = referencePaths;

            }

            return _referencePaths;

        }

    }

}
