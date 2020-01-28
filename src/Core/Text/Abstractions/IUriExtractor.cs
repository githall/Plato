using System;
using System.Collections.Generic;

namespace PlatoCore.Text.Abstractions
{
    public interface IUriExtractor
    {

        string BaseUrl { get; set; }

        IEnumerable<Uri> Extract(string html);

    }

}
