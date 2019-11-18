using Microsoft.AspNetCore.Http;

namespace Plato.GitHub.Models
{
    public class GitHubAuthenticationOptions
    {

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public PathString CallbackPath { get; set; }

    }

}
