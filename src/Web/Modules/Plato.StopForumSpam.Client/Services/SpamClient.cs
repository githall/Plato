﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using PlatoCore.Cache.Abstractions;
using PlatoCore.Http.Abstractions;
using Plato.StopForumSpam.Client.Models;

namespace Plato.StopForumSpam.Client.Services
{

    public class SpamClient : ISpamClient
    {
        private const string ByFormat = "json";

        public ClientOptions Options { get; private set; } = new ClientOptions();

        private readonly IHttpClient _httpClient;
        private readonly ICacheManager _cacheManager;
        
        public SpamClient(IHttpClient httpClient, ICacheManager cacheManager)
        {
            _httpClient = httpClient;
            _cacheManager = cacheManager;
        }

        public void Configure(Action<ClientOptions> configure)
        {
            configure(this.Options);
        }

        public async Task<Response> CheckEmailAddressAsync(string emailAddress)
        {
            return await this.CheckAsync(new {email = emailAddress});
        }

        public async Task<Response> CheckUsernameAsync(string username)
        {
            return await this.CheckAsync(new {username = username});
        }

        public async Task<Response> CheckIpAddressAsync(string ipAddress)
        {
            if (!IPAddress.TryParse(ipAddress, out var address))
            {
                throw new ArgumentException("The ipAddress argument is not a valid IP address.");
            }

            return await this.CheckAsync(new {ip = address.ToString()});
        }

        public async Task<Response> CheckIpAddressAsync(IPAddress ipAddress)
        {
            return await this.CheckAsync(new {ip = ipAddress});
        }

        public async Task<Response> CheckAsync(string username, string emailAddress, string ipAddress)
        {
            IPAddress address = null;
            if (!string.IsNullOrWhiteSpace(ipAddress) && !IPAddress.TryParse(ipAddress, out address))
            {
                throw new ArgumentException("The ipAddress argument (" + ipAddress + ") is not a valid IP address.");
            }

            return await this.CheckAsync(username, emailAddress, address);
        }
        
        private async Task<Response> CheckAsync(object values)
        {

            var parameters = new Dictionary<string, string>();
            if (values != null)
            {
                foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(values))
                {
                    parameters.Add(property.Name, property.GetValue(values)?.ToString());
                }
            }

            return await CheckAsync(parameters);

        }

        public async Task<Response> CheckAsync(string username, string emailAddress, IPAddress ipAddress)
        {
            
            var parameters = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(username))
            {
                parameters.Add("username", username);
            }

            if (!string.IsNullOrWhiteSpace(emailAddress))
            {
                parameters.Add("email", emailAddress);
            }

            if (ipAddress != null)
            {
                parameters.Add("ip", ipAddress.ToString());
            }

            return await this.CheckAsync(parameters);
            
        }

        async Task<Response> CheckAsync(IDictionary<string, string> parameters)
        {

            // We always need params
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            // Ensure format is specified
            if (!parameters.ContainsKey("f"))
            {
                parameters.Add("f", ByFormat);
            }
            
            // Avoiding LINQ here intentionally
            var sb = new StringBuilder();
            foreach (var p in parameters)
            {
                sb.Append(p.Value).Append(",");
            }

            // Store result in cache for a short period of time to avoid multiple http requests
            var token = _cacheManager.GetOrCreateToken(this.GetType(), sb.ToString().TrimEnd(','));
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {
                
                // Make request
                var result = await _httpClient.GetAsync(new Uri("http://www.stopforumspam.com/api"), parameters);
                if (result.Succeeded)
                {
                    return Response.Parse(result.Response, ByFormat);
                }

                // Return failure as no response was received
                return new FailResponse(result.Response, ByFormat)
                {
                    Error = result.Error
                };

            });

        }

        public async Task<Response> AddSpammerAsync(string username, string emailAddress, string ipAddress)
        {
            
            // Validate

            if (string.IsNullOrEmpty(username))
            {
                return new FailResponse(string.Empty, string.Empty)
                {
                    Error = "A username is required!"
                };
            }

            if (string.IsNullOrEmpty(emailAddress))
            {
                return new FailResponse(string.Empty, string.Empty)
                {
                    Error = "An email address is required!"
                };
            }

            if (string.IsNullOrEmpty(ipAddress))
            {
                return new FailResponse(string.Empty, string.Empty)
                {
                    Error = "An IP address is required!"
                };
            }

            // Validate the IP address

            var ok = IPAddress.TryParse(ipAddress, out var address);
            if (!ok)
            {
                return new FailResponse(string.Empty, string.Empty)
                {
                    Error = "The supplied IP address is invalid!"
                };
            }
            
            return await this.AddSpammerAsync(username, emailAddress, address);
         
        }

        public async Task<Response> AddSpammerAsync(string username, string emailAddress, IPAddress ipAddress)
        {

            if (this.Options == null)
            {
                throw new ArgumentNullException(nameof(this.Options));
            }

            if (string.IsNullOrWhiteSpace(this.Options.ApiKey))
            {
                throw new ArgumentNullException(nameof(this.Options.ApiKey));
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException(nameof(username));
            }

            if (string.IsNullOrWhiteSpace(emailAddress))
            {
                throw new ArgumentNullException(nameof(emailAddress));
            }

            var parameters = new Dictionary<string, string>
            {
                {"username", username},
                {"ip_addr", ipAddress.ToString()},
                {"email", emailAddress},
                {"api_key", this.Options.ApiKey}
            };

            // Attempt to post
            var result = await _httpClient.PostAsync(new Uri("http://www.stopforumspam.com/add.php"), parameters);
            if (result.Succeeded)
            {
                
                // Clear cache
                _cacheManager.CancelTokens(this.GetType());

                // Build parsed response
                return Response.Parse(result.Response, ByFormat);

            }
             
            // Return failure as no response was received
            return new FailResponse(result.Response, ByFormat)
            {
                Error = result.Error
            };

        }

    }
    
}
