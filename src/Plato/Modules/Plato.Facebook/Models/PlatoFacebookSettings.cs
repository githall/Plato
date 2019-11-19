using Microsoft.AspNetCore.Http;
using Plato.Internal.Abstractions;

namespace Plato.Facebook.Models
{
    public class PlatoFacebookSettings : Serializable
    {

        public string AppId { get; set; }

        public string AppSecret { get; set; }

        public PathString CallbackPath { get; set; }

    }

}
