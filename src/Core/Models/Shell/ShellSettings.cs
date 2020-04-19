using System;
using System.Text;
using System.Collections.Generic;
using PlatoCore.Abstractions.Extensions;

namespace PlatoCore.Models.Shell
{

    public class ShellSettings : IShellSettings
    {

        private readonly IDictionary<string, string> _values;

        public ShellSettings() : this(new Dictionary<string, string>()) { }

        public ShellSettings(IDictionary<string, string> configuration)
        {
            _values = new Dictionary<string, string>(configuration);

            if (configuration.ContainsKey("State") )
            {
                if (!Enum.TryParse(configuration["State"], true, out TenantState state))
                {
                    this["State"] = TenantState.Invalid.ToString();
                }
            }

        }

        public IDictionary<string, string> Configuration => _values;

        public string this[string key]
        {
            get => _values.TryGetValue(key, out var retVal) ? retVal : null;
            set => _values[key] = value;
        }

        public IEnumerable<string> Keys => _values.Keys;

        public string Name
        {
            get => this["Name"] ?? "";
            set => this["Name"] = value;
        }

        public string OwnerId
        {
            get => this["OwnerId"];
            set => this["OwnerId"] = value;
        }

        public string Location
        {
            get => this["Location"] ?? "";
            set => this["Location"] = value;
        }

        public string DatabaseProvider
        {
            get => this["DatabaseProvider"];
            set => _values["DatabaseProvider"] = value;
        }

        public string ConnectionString
        {
            get => this["ConnectionString"];
            set => _values["ConnectionString"] = value;
        }

        public string TablePrefix
        {
            get => this["TablePrefix"];
            set => _values["TablePrefix"] = value;
        }

        public string RequestedUrlPrefix
        {
            get => this["RequestedUrlPrefix"];
            set => _values["RequestedUrlPrefix"] = value;
        }

        public string RequestedUrlHost
        {
            get => this["RequestedUrlHost"];
            set => this["RequestedUrlHost"] = value;
        }

        public string Theme
        {
            get => this["Theme"];
            set => _values["Theme"] = value;
        }

        public string AuthCookieName
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var c in this.Name)
                {
                    if ((char.IsLetter(c)) || ((char.IsNumber(c))))
                        sb.Append(c);
                }
                return !string.IsNullOrEmpty(sb.ToString()) 
                    ? sb.ToString() 
                    : this.Name;
            }
        }

        public TenantState State
        {
            get
            {
                if (Enum.TryParse(this["State"], true, out TenantState state))
                {
                    return state;
                }
                return TenantState.Uninitialized;
            }
            set
            {            
                this["State"] = value.ToString();
            }
        }

        public DateTimeOffset? CreatedDate
        {
            get
            {
                if (DateTime.TryParse(this["CreatedDate"], out var date))
                {
                    return date;
                }
                return null; 
            }
            set => this["CreatedDate"] = value.HasValue 
                ? value.Value.ToSortableDateTimePattern()
                : string.Empty;
        }

        public DateTimeOffset? ModifiedDate
        {
            get
            {
                if (DateTime.TryParse(this["ModifiedDate"], out var date))
                {
                    return date;
                }
                return null;
            }
            set => this["ModifiedDate"] = value.HasValue
                ? value.Value.ToSortableDateTimePattern()
                : string.Empty;
        }

    }

}
