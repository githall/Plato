using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.Extensions.Hosting;
using PlatoCore.Models.Modules;
using PlatoCore.Modules.Abstractions;
using PlatoCore.Modules.Models;

namespace PlatoCore.Modules
{

    public class ModuleEmbeddedStaticFileProvider : IFileProvider
    {

        // TODO: Look at reducing allocations in this class

        private readonly IModuleManager _moduleManager;

        // A array of folders that can contain static files 
        // The default "wwwroot" folder is handled separately
        private readonly string[] _staticFolders = new string[]
            {
                "/Sites",
                "/Themes",
                "/Locales"
            };

        private IList<IModuleEntry> _modules;
        private readonly string _moduleRoot;
        private readonly string _root;

        public ModuleEmbeddedStaticFileProvider(IHostEnvironment eng, IServiceProvider services)
        {
            var moduleOptions = services.GetRequiredService<IOptions<ModuleOptions>>();
            _moduleManager = services.GetRequiredService<IModuleManager>();                        
            _moduleRoot = moduleOptions.Value.VirtualPathToModulesFolder;
            _root = eng.ContentRootPath + "\\";
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            return NotFoundDirectoryContents.Singleton;
        }

        public IFileInfo GetFileInfo(string subpath)
        {

            if (subpath == null)
            {
                return new NotFoundFileInfo(subpath);
            }

            if (_modules == null)
            {
                _modules = _moduleManager.LoadModulesAsync()
                    .GetAwaiter()
                    .GetResult()
                    .ToList();
            }

            var path = NormalizePath(subpath);
            var index = path.IndexOf('/');

            // "/**/*.*".
            if (index != -1)
            {

                // Resolve the module id.
                var module = path.Substring(0, index);

                // Check if it is a module request
                if (_modules.Any(m => m.Descriptor.Id.Equals(module, StringComparison.OrdinalIgnoreCase)))
                {

                    // Resolve the embedded file sub path: "Content/**/*.*"
                    var fileSubPath = path.Substring(index + 1).Replace("/", "\\");

                    // We only serve static files from the modules "Content" directory
                    if (fileSubPath.StartsWith("Content", StringComparison.OrdinalIgnoreCase))
                    {
                        return new PhysicalFileInfo(new FileInfo(_root + Path.Combine(
                                _moduleRoot,
                                module,
                                fileSubPath)));
                    }

                }
                else
                {

                    var fileSubPath = subpath.Replace("/", "\\");
                    var isCustomStaticFolder = false;
                    foreach (var staticFolder in _staticFolders)
                    {
                        isCustomStaticFolder = subpath.StartsWith(staticFolder, StringComparison.OrdinalIgnoreCase);
                        if (isCustomStaticFolder)
                        {
                            break;
                        }
                    }

                    if (isCustomStaticFolder)
                    {
                        return new PhysicalFileInfo(new FileInfo(_root + fileSubPath));
                    }
                    else
                    {
                        var filePath = _root + "wwwroot" + fileSubPath;
                        return new PhysicalFileInfo(new FileInfo(filePath));
                    }

                }

            }
            else
            {
                // Accommodate for files inside the root of the "wwwroot" folder
                // I.e. favicon.ico, security.txt + others
                return new PhysicalFileInfo(new FileInfo(_root + Path.Combine("wwwroot", path)));
            }

            return new NotFoundFileInfo(subpath);

        }

        public IChangeToken Watch(string filter)
        {
            return NullChangeToken.Singleton;
        }

        private string NormalizePath(string path)
        {
            return path.Replace('\\', '/').Trim('/').Replace("//", "/");
        }

    }

}
