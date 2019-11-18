using Microsoft.AspNetCore.Http;
using Plato.Internal.Abstractions;

namespace Plato.GitHub.Models
{
    public class GitHubSettings : Serializable
    {

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public PathString CallbackPath { get; set; }

    }

}
