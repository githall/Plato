using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PlatoCore.Net.Abstractions
{

    public interface IHttpClient
    {

        int Timeout { get; set; }

        Task<HttpClientResponse> GetAsync(string url);

        Task<HttpClientResponse> GetAsync(Uri url);

        Task<HttpClientResponse> GetAsync(Uri url, IDictionary<string, string> parameters);

        Task<HttpClientResponse> PostAsync(Uri url, IDictionary<string, string> parameters);

        Task<HttpClientResponse> PostAsync(Uri url, string data);

        Task<HttpClientResponse> RequestAsync(HttpMethod method, Uri url, IDictionary<string, string> parameters);

        Task<HttpClientResponse> RequestAsync(HttpMethod method, Uri url, string data);

        Task<HttpClientResponse> RequestAsync(HttpMethod method, Uri url, IDictionary<string, string> parameters, string contentType);

        Task<HttpClientResponse> RequestAsync(HttpMethod method, Uri url, IDictionary<string, string> parameters, string data, string contentType);

    }

    public class HttpClientResponse
    {
        public string Response { get; set; }
        
        public bool Succeeded { get; set; }

        public string Error { get; set; }

    }

}
