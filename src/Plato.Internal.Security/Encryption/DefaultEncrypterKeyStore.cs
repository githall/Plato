using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plato.Internal.Abstractions.Settings;
using Plato.Internal.Security.Abstractions.Encryption;

namespace Plato.Internal.Security.Encryption
{

    public class DefaultEncrypterKeyStore : IEncrypterKeyStore
    {

        private const string KeyFileName = "plato-keys.txt";
        private const string VectorSection = "vector";
        private const string KeySection = "key";
        private const string Separator = ":";

        private readonly ILogger<DefaultEncrypterKeyStore> _logger;
        private readonly IEncrypterKeyBuilder _keyBuilder;
        private readonly PlatoOptions _platoOptions;

        public DefaultEncrypterKeyStore(
            IOptions<PlatoOptions> platoOptionsAccessor,            
            ILogger<DefaultEncrypterKeyStore> logger,
            IEncrypterKeyBuilder keyBuilder)
        {
            _platoOptions = platoOptionsAccessor.Value;
            _keyBuilder = keyBuilder;
            _logger = logger;
        }

        public async Task<EncrypterKeys> GetOrCreateKeysAsync()
        {
            return await GetOrCreateKeysInternalAsync();
        }

        async Task<EncrypterKeys> GetOrCreateKeysInternalAsync()
        {

            // No secrets path specified, return default keys
            if (string.IsNullOrEmpty(_platoOptions.SecretsPath))
            {
                return DefaultKeys();
            }

            // Build path to persistent key store
            var path = Path.Combine(_platoOptions.SecretsPath, KeyFileName);
            
            // Store does not exist 
            if (!File.Exists(path))
            {
                // Create one
                var newKeys = await CreateKeyFileAsync(path);
                if (!string.IsNullOrEmpty(newKeys))
                {
                    return ParseKeys(newKeys);
                }                
            }
            
            // Attempt to read any existing key file
            var keys = await ReadFileAsync(path);
            if (!string.IsNullOrEmpty(keys))
            {
                // Store exists and we have existing keys
                return ParseKeys(keys);
            }
            else 
            {
                // Store exists but no keys are present
                keys = await CreateKeyFileAsync(path);
                // Ensure we could add new keys
                if (!string.IsNullOrEmpty(keys))
                {
                    return ParseKeys(keys);
                }                
            }

            // Just return default keys if anything went wrong
            return DefaultKeys();            

        }

        async Task<string> CreateKeyFileAsync(string path)
        {

            // Build keys
            var keys = BuildKeys(_keyBuilder.Key, _keyBuilder.Vector);

            // Attempt to persist keys
            var success = true;
            try
            {
                await SaveFileAsync(path, keys);
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation($"Plato encryption keys successfully generated and persisted to {path}.");
                }
            }
            catch (Exception e)
            {
                success = false;
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError(e, $"An error occurred whilst attempting to persist your Plato encryption keys to {path}. The following error occurred: {e.Message}");
                }
            }

            // Ensure no problems occurred
            if (success)
            {
                // Attempt to read the newly created key file to 
                // ensure it was created and can be read successfully
                if (File.Exists(path))
                {
                    return await ReadFileAsync(path);
                }
            }

            return string.Empty;

        }

        EncrypterKeys DefaultKeys()
        {
            return ParseKeys(BuildDefaultKeys());
        }

        string BuildDefaultKeys()
        {
            // The default keys are only ever used if no secrets path is
            // specified (i.e. in development) or if errors occur 
            // whilst persisting or loading the keys
            return BuildKeys("KsGwDJpG2qW51PmoWy2xLg34egR/z6gITbjN3mBSgs4=", "KElBLMef/AXJx+ce3f3szA==");
        }

        string BuildKeys(string key, string vector)
        {

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (string.IsNullOrEmpty(vector))
            {
                throw new ArgumentNullException(nameof(vector));
            }

            var sb = new StringBuilder();
            sb
                .Append(KeySection)
                .Append(Separator)
                .Append(key)
                .Append(Environment.NewLine)
                .Append(VectorSection)
                .Append(Separator)
                .Append(vector);

            return sb.ToString();

        }

        EncrypterKeys ParseKeys(string input)
        {

            var dict = new Dictionary<string, string>();
            using (var reader = new StringReader(input))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var field = line.Split(new[] { Separator }, 2, StringSplitOptions.None);
                    var fieldLength = field.Length;
                    if (fieldLength != 2)
                        continue;
                    for (var i = 0; i < fieldLength; i++)
                    {
                        field[i] = field[i].Trim();
                    }
                    switch (field[0].ToLowerInvariant())
                    {
                        case KeySection:
                            dict.Add(KeySection, field[1]);
                            break;
                        case VectorSection:
                            dict.Add(VectorSection, field[1]);
                            break;

                    }
                }
            }

            return new EncrypterKeys()
            {
                Key = dict.ContainsKey(KeySection) ? dict[KeySection] : throw new Exception($"A problem occurred parsing the 'Key' parameter within {Path.Combine(_platoOptions.SecretsPath, KeyFileName)}"),
                Vector = dict.ContainsKey(VectorSection) ? dict[VectorSection] : throw new Exception($"A problem occurred parsing the 'Vector' parameter within {Path.Combine(_platoOptions.SecretsPath, KeyFileName)}")
            };

        }

        async Task<string> ReadFileAsync(string path)
        {              
            using (var reader = File.OpenText(path))
            {
                return await reader.ReadToEndAsync();
            }
        }

        async Task SaveFileAsync(string path, string content)
        {
            using (var writer = File.CreateText(path))
            {
                await writer.WriteAsync(content);
            }
        }

    }

}
