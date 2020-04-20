using System;
using System.Collections.Generic;

namespace PlatoCore.Models.Shell
{
    public interface IShellSettings
    {

        string Name { get; set; }

        string Location { get; set; }

        string ConnectionString { get; set; }

        /// <summary>
        /// Unique database table prefix for the shell.
        /// </summary>
        string TablePrefix { get; set; }

        string DatabaseProvider { get; set; }

        /// <summary>
        /// For example https://site1.url.com/, https://site2.url.com/, https://site3.url.com/ etc
        /// </summary>
        string RequestedUrlHost { get; set; }

        /// <summary>
        /// For example https://url.com/site1, https://url.com/site2, https://url.com/site3, etc
        /// </summary>
        string RequestedUrlPrefix { get; set; }
     
        /// <summary>
        /// An identifier for this tenant. For example an email address or primary key value. 
        /// </summary>
        string OwnerId { get; set; }

        /// <summary>
        /// The default theme for the shell.
        /// </summary>
        string Theme { get; set; }

        string AuthCookieName { get; }

        /// <summary>
        /// The shell state. 
        /// </summary>
        TenantState State { get; set; }

        DateTimeOffset? CreatedDate { get; set; }


        DateTimeOffset?ModifiedDate { get; set; }

        string this[string key] { get; }

        IEnumerable<string> Keys { get; }

        IDictionary<string, string> Configuration { get; }

    }

}
