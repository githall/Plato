using Microsoft.AspNetCore.Http;

namespace Plato.Facebook.Models
{

    public class FacebookAuthenticationOptions
    {

        public string AppId { get; set; }

        public string AppSecret { get; set; }

        public PathString CallbackPath { get; set; }

    }

}
