using Microsoft.AspNetCore.Http;
using Plato.Internal.Abstractions;

namespace Plato.Google.Models
{
    public class PlatoGoogleSettings : Serializable
    {

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public PathString CallbackPath { get; set; }

    }

}
