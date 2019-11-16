using Microsoft.AspNetCore.Http;

namespace Plato.Google.Models
{
    public class GoogleAuthenticationOptions
    {

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public PathString CallbackPath { get; set; }

    }

}
