﻿using System.Collections.Generic;
using PlatoCore.Assets.Abstractions;

namespace Plato.Media.LightBox.Assets
{
    public class AssetProvider : IAssetProvider
    {

        public IEnumerable<AssetEnvironment> GetAssetEnvironments()
        {

            var constraints = new AssetConstraints()
            {
                Routes = new List<AssetConstraint>() {
                    new AssetConstraint()
                    {
                        ["area"] = "Plato.*",
                        ["controller"] = "Home",
                        ["action"] = "Display",
                    },
                    new AssetConstraint()
                    {
                        ["area"] = "Plato.*",
                        ["controller"] = "Home",
                        ["action"] = "Create",
                    },
                    new AssetConstraint()
                    {
                        ["area"] = "Plato.*",
                        ["controller"] = "Home",
                        ["action"] = "Edit",
                    }
                }
            };

            return new List<AssetEnvironment>
            {

                // Development
                new AssetEnvironment(TargetEnvironment.Development, new List<Asset>()
                {
                    new Asset()
                    {
                        Url = "/plato.media.lightbox/content/js/lightbox.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer,
                        Constraints = constraints
                    },
                    new Asset()
                    {
                        Url = "/plato.media.lightbox/content/css/lightbox.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header,
                        Constraints = constraints
                    }
                }),

                // Staging
                new AssetEnvironment(TargetEnvironment.Staging, new List<Asset>()
                {
                    new Asset()
                    {
                        Url = "/plato.media.lightbox/content/js/lightbox.min.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer,
                        Constraints = constraints
                    },
                    new Asset()
                    {
                        Url = "/plato.media.lightbox/content/css/lightbox.min.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header,
                        Constraints = constraints
                    }
                }),

                // Production
                new AssetEnvironment(TargetEnvironment.Production, new List<Asset>()
                {
                    new Asset()
                    {
                        Url = "/plato.media.lightbox/content/js/lightbox.min.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer,
                        Constraints = constraints
                    },
                    new Asset()
                    {
                        Url = "/plato.media.lightbox/content/css/lightbox.min.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header,
                        Constraints = constraints
                    }
                })

            };

        }

    }

}
