using System;
using System.Threading.Tasks;
using Plato.WebApi.Models;
using PlatoCore.Hosting.Web.Abstractions;

namespace Plato.WebApi.Services
{

    public class WebApiOptionsFactory : IWebApiOptionsFactory
    {

        private readonly IContextFacade _contextFacade;     

        public WebApiOptionsFactory(IContextFacade contextFacade)
        {
            _contextFacade = contextFacade;    
        }

        public async Task<WebApiOptions> GetSettingsAsync()
        {
            return new WebApiOptions()
            {
                Url = await GetUrl(),
                ApiKey = await GetApiKey()
            };
        }

        async Task<string> GetUrl()
        {
            return await _contextFacade.GetBaseUrlAsync();
        }

        async Task<string> GetApiKey()
        {

            var settings = await _contextFacade.GetSiteSettingsAsync();

            if (settings == null)
            {
                return string.Empty;
            }

            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return settings.ApiKey;
            }

            if (String.IsNullOrWhiteSpace(user.ApiKey))
            {
                return settings.ApiKey;
            }

            return $"{settings.ApiKey}:{user.ApiKey}";

        }

    }

}
