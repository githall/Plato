using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plato.Internal.Abstractions.Settings;
using Plato.Internal.Security.Abstractions.Encryption;

namespace Plato.Internal.Security.Encryption
{

    public class PlatoKeyStore : IPlatoKeyStore
    {

        private const string KeySection = "key";
        private const string VectorSection = "vector";

        private const string DefaultKeys = @"
            Key: 45BLO2yoJkvBwz99kBEMlNkxvL40vUSGaqr/WBu3+Vg=
            Vector: Ou3fn+I9SVicGWMLkFEgZQ==";

        private const string KeyFile = "plato-keys.txt";

        private readonly ILogger<PlatoKeyStore> _logger;
        
        private readonly PlatoOptions _platoOptions;

        public PlatoKeyStore(
            IOptions<PlatoOptions> platoOptionsAccessor,
            ILogger<PlatoKeyStore> logger)
        {
            _platoOptions = platoOptionsAccessor.Value;            
            _logger = logger;
        }

        public async Task<PlatoKeys> GetOrCreateKeysAsync()
        {              

            // No secrets path specified, return default keys
            if (string.IsNullOrEmpty(_platoOptions.SecretsPath))
            {
                return Parse(DefaultKeys);
            }

            // Build path to persistent key file
            var path = Path.Combine(_platoOptions.SecretsPath, KeyFile);

            // Does the key file exists?
            var text = await ReadFileAsync(path);
            if (string.IsNullOrEmpty(text))
            {

                text = @"
                    Key: {key}
                    Vector: {vector}
                ";

                text = text.Replace("{key}", System.Guid.NewGuid().ToString());
                text = text.Replace("{vector}", System.Guid.NewGuid().ToString());

                try                
                {

                    await SaveFileAsync(path, text);

                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogInformation($"Plato encryption keys successfully generated and persisted to {path}.");
                    }

                } 
                catch (Exception e)
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                    {
                        _logger.LogError(e, $"An error occurred whilst attempting to persist your Plato encryption keys to {path}. The following error occurred: {e.Message}");
                    }
                }                

                return Parse(text);

            }

            return Parse(DefaultKeys);            

        }

        PlatoKeys Parse(string input)
        {

            var dict = new Dictionary<string, string>();
            using (var reader = new StringReader(input))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var field = line.Split(new[] { ":" }, 2, StringSplitOptions.None);
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

            return new PlatoKeys()
            {
                Key = dict.ContainsKey(KeySection) ? dict[KeySection] : throw new Exception($"A problem occurred parsing the key parameter within {Path.Combine(_platoOptions.SecretsPath, KeyFile)}"),
                Vector = dict.ContainsKey(VectorSection) ? dict[VectorSection] : throw new Exception($"A problem occurred parsing the vector parameter within {Path.Combine(_platoOptions.SecretsPath, KeyFile)}")
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
