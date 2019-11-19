using Microsoft.AspNetCore.Http;
using Plato.Internal.Abstractions;

namespace Plato.Twitter.Models
{
    public class PlatoTwitterSettings : Serializable
    {

        public string ConsumerKey { get; set; }

        public string ConsumerSecret { get; set; }

        public PathString CallbackPath { get; set; }

        public string AccessToken { get; set; }

        public string AccessTokenSecret { get; set; }

    }
}
