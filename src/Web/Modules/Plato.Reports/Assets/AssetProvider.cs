using System.Collections.Generic;
using System.Threading.Tasks;
using PlatoCore.Assets.Abstractions;

namespace Plato.Reports.Assets
{
    public class AssetProvider : IAssetProvider
    {
        
        public IEnumerable<AssetEnvironment> GetAssetEnvironments()
        {

            return new List<AssetEnvironment>
            {

                // Development
                new AssetEnvironment(TargetEnvironment.Development, new List<Asset>()
                {
                    new Asset()
                    {
                        Url = "/plato.reports/content/css/vendors/chart.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header,
                        Constraints = new AssetConstraints()
                        {
                            Layout = "_AdminLayout"
                        }
                    },
                    new Asset()
                    {
                        Url = "/plato.reports/content/css/reports.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header,
                        Constraints = new AssetConstraints()
                        {
                            Layout = "_AdminLayout"
                        }
                    },
                    new Asset()
                    {
                        Url = "/plato.reports/content/js/vendors/chart.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer,
                        Constraints = new AssetConstraints()
                        {
                            Layout = "_AdminLayout"
                        }
                    },
                    new Asset()
                    {
                        Url = "/plato.reports/content/js/reports.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer,
                        Constraints = new AssetConstraints()
                        {
                            Layout = "_AdminLayout"
                        }
                    }
                }),

                // Staging
                new AssetEnvironment(TargetEnvironment.Staging, new List<Asset>()
                {
                    new Asset()
                    {
                        Url = "/plato.reports/content/css/vendors/chart.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header,
                        Constraints = new AssetConstraints()
                        {
                            Layout = "_AdminLayout"
                        }
                    },
                    new Asset()
                    {
                        Url = "/plato.reports/content/css/reports.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header,
                        Constraints = new AssetConstraints()
                        {
                            Layout = "_AdminLayout"
                        }
                    },
                    new Asset()
                    {
                        Url = "/plato.reports/content/js/vendors/chart.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer,
                        Constraints = new AssetConstraints()
                        {
                            Layout = "_AdminLayout"
                        }
                    },
                    new Asset()
                    {
                        Url = "/plato.reports/content/js/reports.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer,
                        Constraints = new AssetConstraints()
                        {
                            Layout = "_AdminLayout"
                        }
                    }
                }),

                // Production
                new AssetEnvironment(TargetEnvironment.Production, new List<Asset>()
                {
                    new Asset()
                    {
                        Url = "/plato.reports/content/css/vendors/chart.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header,
                       Constraints = new AssetConstraints()
                        {
                            Layout = "_AdminLayout"
                        }
                    },
                    new Asset()
                    {
                        Url = "/plato.reports/content/css/reports.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header,
                        Constraints = new AssetConstraints()
                        {
                            Layout = "_AdminLayout"
                        }
                    },
                    new Asset()
                    {
                        Url = "/plato.reports/content/js/vendors/chart.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer,
                        Constraints = new AssetConstraints()
                        {
                            Layout = "_AdminLayout"
                        }
                    },
                    new Asset()
                    {
                        Url = "/plato.reports/content/js/reports.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer,
                        Constraints = new AssetConstraints()
                        {
                            Layout = "_AdminLayout"
                        }
                    }
                })

            };

        }

    }

}
