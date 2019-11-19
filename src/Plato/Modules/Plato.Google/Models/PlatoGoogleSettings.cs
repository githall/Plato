using Microsoft.AspNetCore.Http;
using Plato.Internal.Abstractions;

namespace Plato.Google.Models
{
    public class PlatoGoogleSettings : Serializable
    {

        // Authentication

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public PathString CallbackPath { get; set; }

        // Analytics

        public string TrackingId { get; set; }

    }

}
