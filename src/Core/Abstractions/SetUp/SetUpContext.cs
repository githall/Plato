using System;
using System.Collections.Generic;
using System.Text;

namespace PlatoCore.Abstractions.SetUp
{
    public interface ISetUpContext
    {

        string SiteName { get; set; }

        string DatabaseProvider { get; set; }

        string DatabaseConnectionString { get; set; }

        string DatabaseTablePrefix { get; set; }

        string AdminEmail { get; set; }

        string AdminUsername { get; set; }

        string AdminPassword { get; set; }


        string RequestedUrlPrefix { get; set; }

        string RequestedUrlHost { get; set; }

        bool IsHost { get; set; }

        IDictionary<string, string> Errors { get; set; }

    }

    public class SetUpContext : ISetUpContext
    {

        public string SiteName { get; set; }

        public string DatabaseProvider { get; set; }

        public string DatabaseConnectionString { get; set; }

        public string DatabaseTablePrefix { get; set; }

        public string AdminEmail { get; set; }

        public string AdminUsername { get; set; }

        public string AdminPassword { get; set; }

        public string RequestedUrlPrefix { get; set; }

        public string RequestedUrlHost { get; set; }

        public bool IsHost { get; set; }

        public IDictionary<string, string> Errors { get; set; }

    }

}
