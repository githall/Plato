using Microsoft.AspNetCore.Http;
using Plato.Internal.Abstractions;

namespace Plato.GitHub.Models
{
    public class PlatoGitHubSettings : Serializable
    {

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public PathString CallbackPath { get; set; }

    }

}
