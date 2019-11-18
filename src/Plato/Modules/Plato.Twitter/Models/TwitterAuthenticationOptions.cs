using Microsoft.AspNetCore.Http;

namespace Plato.Twitter.Models
{
    public class TwitterAuthenticationOptions
    {

        public string ConsumerKey { get; set; }

        public string ConsumerSecret { get; set; }

        public PathString CallbackPath { get; set; }

        public string AccessToken { get; set; }

        public string AccessTokenSecret { get; set; }
        
    }

}
