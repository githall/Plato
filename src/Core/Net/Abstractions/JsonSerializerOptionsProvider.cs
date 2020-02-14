using System.Text.Json;

namespace PlatoCore.Net.Abstractions
{
    internal static class JsonSerializerOptionsProvider
    {
        public static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
        };
    }
}
