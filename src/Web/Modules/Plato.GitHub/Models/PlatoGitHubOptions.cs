using Microsoft.AspNetCore.Http;

namespace Plato.GitHub.Models
{
    public class PlatoGitHubOptions
    {

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public PathString CallbackPath { get; set; }

    }

}
