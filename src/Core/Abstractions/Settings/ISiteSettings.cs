﻿using PlatoCore.Abstractions.Routing;

namespace PlatoCore.Abstractions.Settings
{
    public interface ISiteSettings : ISerializable
    {

        string SiteName { get; set; }

        string SiteSalt { get; set; }
        
        string PageTitleSeparator { get; set; }

        string SuperUser { get; set; }

        string Culture { get; set; } 

        string Calendar { get; set; }

        string TimeZone { get; set; }

        string DateTimeFormat { get; set; }
        
        bool UseCdn { get; set; }

        int PageSize { get; set; }

        int MaxPageSize { get; set; }

        int MaxPagedCount { get; set; }

        string BaseUrl { get; set; }

        HomeRoute HomeRoute { get; set; }

        string HomeAlias { get; set; }

        string Theme { get; set; }

        string ApiKey { get; set; }

    }

}
