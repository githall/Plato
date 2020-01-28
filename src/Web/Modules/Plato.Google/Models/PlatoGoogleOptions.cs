using Microsoft.AspNetCore.Http;

namespace Plato.Google.Models
{
    public class PlatoGoogleOptions
    {

        // Authentication

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public PathString CallbackPath { get; set; }

        // Analytics

        public string TrackingId { get; set; }

    }

}
