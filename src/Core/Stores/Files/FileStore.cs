using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using PlatoCore.FileSystem.Abstractions;
using PlatoCore.Stores.Abstractions.Files;

namespace PlatoCore.Stores.Files
{

    public class FileStore : IFileStore
    {

        private readonly IPlatoFileSystem _fileSystem;                
        private readonly ILogger<FileStore> _logger;
        private readonly IMemoryCache _memoryCache;

        public FileStore(
            IPlatoFileSystem fileSystem,
            ILogger<FileStore> logger,
            IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _fileSystem = fileSystem;
            _logger = logger;
        }
        
        public async Task<byte[]> GetFileBytesAsync(string path)
        {

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (!_memoryCache.TryGetValue(path, out byte[] result))
            {
                result = await _fileSystem.ReadFileBytesAsync(path);
                if (result != null)
                {
                    _memoryCache.Set(path, result, new MemoryCacheEntryOptions()
                        .AddExpirationToken(_fileSystem.Watch(path)));
                }
            }

            return result;

        }
        
        public async Task<string> GetFileAsync(string path)
        {

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (!_memoryCache.TryGetValue(path, out string result))
            {
                result = await _fileSystem.ReadFileAsync(path);
                if (result != null)
                {
                    _memoryCache.Set(path, result, new MemoryCacheEntryOptions()
                        .AddExpirationToken(_fileSystem.Watch(path)));
                }
            }

            return result;

        }

        public string Combine(params string[] paths)
        {
            return _fileSystem.Combine(paths);
        }

    }

}