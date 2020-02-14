using System;
using System.Text.Json;
using System.Threading.Tasks;
using PlatoCore.Net.Abstractions;

namespace PlatoCore.Net.Extensions
{
    public static class HttpClientExtensions
    {

        public static async Task<T> GetJsonAsync<T>(
            this IHttpClient httpClient,
            string requestUri)
        {
            var response = await httpClient.GetAsync(requestUri);
            return JsonSerializer.Deserialize<T>(response.Response, JsonSerializerOptionsProvider.Options);
        }

        public static async Task<T> PostJsonAsync<T>(
            this IHttpClient httpClient,            
            string requestUri, 
            object content)
        {
            var uri = new Uri(requestUri);
            var json = JsonSerializer.Serialize(content, JsonSerializerOptionsProvider.Options);
            var response = await httpClient.PostAsync(uri, json);         
            return JsonSerializer.Deserialize<T>(response.Response, JsonSerializerOptionsProvider.Options);        
        }

    }

}
