using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Internal;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using PlatoCore.Modules.Abstractions;
using PlatoCore.Modules.Models;

namespace PlatoCore.Modules
{

    public class ModuleViewFileProvider : IFileProvider
    {
        private readonly string _moduleRoot;
        private readonly string _root;

        public ModuleViewFileProvider(IServiceProvider services)
        {
            var env = services.GetRequiredService<IHostEnvironment>();
            var moduleOptions = services.GetRequiredService<IOptions<ModuleOptions>>();
            services.GetRequiredService<IModuleManager>();            
            _moduleRoot = moduleOptions.Value.VirtualPathToModulesFolder + "/";
            _root = env.ContentRootPath + "\\";
        }

        public IDirectoryContents GetDirectoryContents(string path)
        {
          
            if (path == null)
            {
                return NotFoundDirectoryContents.Singleton;
            }

            var folder = NormalizePath(path);

                // Under "Modules/**".
             if (folder.StartsWith(_moduleRoot, StringComparison.Ordinal))
            {
                // Check for a "Pages" or a "Views" segment.
                var tokenizer = new StringTokenizer(folder, new char[] { '/' });
                if (tokenizer.Any(s => s == "Pages" || s == "Views"))
                {
                    // Resolve the subpath relative to the application's module root.
                    var folderSubPath = folder.Substring(_moduleRoot.Length);

                    // And serve the contents from the physical application root folder.
                    return new PhysicalDirectoryContents(_root + folder.Replace("/", "\\"));
                }
            }

            return NotFoundDirectoryContents.Singleton;
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            if (subpath == null)
            {
                return new NotFoundFileInfo(subpath);
            }

            var path = NormalizePath(subpath);

            // "Modules/**/*.*".
            if (path.StartsWith(_moduleRoot, StringComparison.Ordinal))
            {
                // Resolve the subpath relative to the application's module.
                var fileSubPath = path.Substring(_moduleRoot.Length);

                // And serve the file from the physical application root folder.
                return new PhysicalFileInfo(new FileInfo(_root + fileSubPath));
            }

            return new NotFoundFileInfo(subpath);
        }

        public IChangeToken Watch(string filter)
        {
            if (filter == null)
            {
                return NullChangeToken.Singleton;
            }

            var path = NormalizePath(filter);

            // "Areas/{ApplicationName}/**/*.*".
            if (path.StartsWith(_moduleRoot, StringComparison.Ordinal))
            {
                // Resolve the subpath relative to the application's module.
                var fileSubPath = path.Substring(_moduleRoot.Length);

                // And watch the application file from the physical application root folder.
                return new PollingFileChangeToken(new FileInfo(_root + fileSubPath));
            }

            return NullChangeToken.Singleton;
        }

        private string NormalizePath(string path)
        {
            return path.Replace('\\', '/').Trim('/');
        }

    }

}
