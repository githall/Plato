using PlatoCore.Abstractions;

namespace Plato.Site.Models
{
    public class PlatoSiteSettings : Serializable
    {

        private string _hostUrl;
        private string _demoUrl;

        public string HostUrl
        {
            get
            {
                if (!string.IsNullOrEmpty(_hostUrl))
                {
                    if (_hostUrl.EndsWith("/"))
                    {
                        _hostUrl = _hostUrl.Substring(0, _hostUrl.Length - 1);
                    }
                }
                return _hostUrl;
            }
            set
            {
                _hostUrl = value;
            }
        }

        public string DemoUrl {
            get
            {
                if (!string.IsNullOrEmpty(_demoUrl))
                {
                    if (_demoUrl.EndsWith("/"))
                    {
                        _demoUrl = _demoUrl.Substring(0, _demoUrl.Length - 1);
                    }
                }
                return _demoUrl;
            }
            set
            {
                _demoUrl = value;
            }
        }

        public string PlatoDesktopUrl { get; set; }

    }

}
