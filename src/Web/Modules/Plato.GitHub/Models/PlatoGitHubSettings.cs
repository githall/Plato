using Microsoft.AspNetCore.Http;
using PlatoCore.Abstractions;

namespace Plato.GitHub.Models
{
    public class PlatoGitHubSettings : Serializable
    {

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public PathString CallbackPath { get; set; }

    }

}
