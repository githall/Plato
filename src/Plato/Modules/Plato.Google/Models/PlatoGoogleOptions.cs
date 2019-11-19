using Microsoft.AspNetCore.Http;

namespace Plato.Google.Models
{
    public class PlatoGoogleOptions
    {

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public PathString CallbackPath { get; set; }

    }

}
