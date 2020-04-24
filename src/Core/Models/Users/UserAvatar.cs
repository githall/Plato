using System;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Routing;

namespace PlatoCore.Models.Users
{
    [DataContract]
    public class UserAvatar
    {

        private readonly ISimpleUser _user;

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [IgnoreDataMember]
        public RouteValueDictionary DefaultRoute {
            get
            {
                if (_user == null)
                {
                    throw new ArgumentNullException(nameof(_user));
                }

                return new RouteValueDictionary()
                {
                    ["area"] = "Plato.Users",
                    ["controller"] = "Letter",
                    ["action"] = "Get",
                    ["letter"] = _user.DisplayName != null
                        ? _user.DisplayName.ToLower().Substring(0, 1)
                        : "-",
                    ["color"] = _user.PhotoColor
                };

            }
        }

        [IgnoreDataMember]
        public bool HasAvatar { get; }

        public UserAvatar(ISimpleUser user)
        {

            _user = user;

            if (_user.Id <= 0)
            {
                Url = "/images/photo.png";            
            }

            // If we have a photo Url use that
            if (!string.IsNullOrEmpty(_user.PhotoUrl))
            {
                Url = _user.PhotoUrl;             
                HasAvatar = true;               
            }

        }

    }

}
