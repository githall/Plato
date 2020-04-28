﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using PlatoCore.Abstractions;
using PlatoCore.Data.Abstractions;
using PlatoCore.Features.Abstractions;
using PlatoCore.Models.Features;
using Plato.Labels.Models;
using Plato.Labels.Services;
using Plato.Site.Demo.Models;
using Plato.Users.Services;
using PlatoCore.Hosting.Web.Abstractions;

namespace Plato.Site.Demo.Services
{

    public class SampleLabelsService : ISampleLabelsService
    {

        private readonly List<SampleDataDescriptor> Descriptors = new List<SampleDataDescriptor>()
        {
            new SampleDataDescriptor()
            {
                ModuleId = "Plato.Discuss",
                EntityType = "topic"
            },
            new SampleDataDescriptor()
            {
                ModuleId = "Plato.Docs",
                EntityType = "doc",
            },
            new SampleDataDescriptor()
            {
                ModuleId = "Plato.Articles",
                EntityType = "articles",
            },
            new SampleDataDescriptor()
            {
                ModuleId = "Plato.Ideas",
                EntityType = "idea"
            },
            new SampleDataDescriptor()
            {
                ModuleId = "Plato.Issues",
                EntityType = "issue"
            },
            new SampleDataDescriptor()
            {
                ModuleId = "Plato.Questions",
                EntityType = "question"
            }
        };

        private readonly Random _random;

        private readonly ILabelManager<LabelBase> _labelManager;
        private readonly IUserColorProvider _colorProvider;
        private readonly IContextFacade _contextFacade;
        private readonly IFeatureFacade _featureFacade;        
        private readonly IDbHelper _dbHelper;

        public SampleLabelsService(
            ILabelManager<LabelBase> labelManager,
            IUserColorProvider colorProvider,
            IContextFacade contextFacade,
            IFeatureFacade featureFacade,
            IDbHelper dbHelper)
        {            
            _featureFacade = featureFacade;
            _contextFacade = contextFacade;
            _colorProvider = colorProvider;
            _labelManager = labelManager;
            _dbHelper = dbHelper;
            _random = new Random();
        }

        public async Task<ICommandResultBase> InstallAsync()
        {

            var output = new CommandResultBase();
            foreach (var descriptor in Descriptors)
            {

                var feature = await _featureFacade.GetFeatureByIdAsync(descriptor.ModuleId);
                if (feature == null)
                {
                    continue;
                }

                var result = await InstallLabelsAsync(feature);
                if (!result.Succeeded)              
                {
                    return output.Failed(result.Errors.ToArray());
                }

            }

            return output.Success();

        }

        public async Task<ICommandResultBase> UninstallAsync()
        {

            var output = new CommandResultBase();
            foreach (var descriptor in Descriptors)
            {

                // Ensure the feature is enabled
                var feature = await _featureFacade.GetFeatureByIdAsync(descriptor.ModuleId);
                if (feature == null)
                {
                    continue;
                }

                var result = await UninstallLabelsAsync(feature);
                if (!result.Succeeded)
                {
                    return output.Failed(result.Errors.ToArray());
                }

            }

            return output.Success();

        }

        // ----------

        async Task<ICommandResultBase> InstallLabelsAsync(IShellFeature feature)
        {

            // Validate

            if (feature == null)
            {
                throw new ArgumentNullException(nameof(feature));
            }

            if (string.IsNullOrEmpty(feature.ModuleId))
            {
                throw new ArgumentNullException(nameof(feature.ModuleId));
            }

            // Our result
            var result = new CommandResultBase();

            for (var i = 0; i < 10; i++)
            {
                var categoryResult = await InstallLabelAsync(feature, i);
                if (!categoryResult.Succeeded)
                {
                    return result.Failed(categoryResult.Errors.ToArray());
                }
            }

            return result.Success();

        }

        async Task<ICommandResultBase> InstallLabelAsync(IShellFeature feature, int sortOrder = 0)
        {

            // Validate

            if (feature == null)
            {
                throw new ArgumentNullException(nameof(feature));
            }

            if (string.IsNullOrEmpty(feature.ModuleId))
            {
                throw new ArgumentNullException(nameof(feature.ModuleId));
            }

            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Our result
            var result = new CommandResultBase();

            var foreColor = "#ffffff";
            var backColor = $"#{_colorProvider.GetColor()}";

            var categoryResult = await _labelManager.CreateAsync(new LabelBase()
            {
                FeatureId = feature.Id,
                ParentId = 0,
                Name = $"Example Label {_random.Next(0, 2000).ToString()}",
                Description = $"This is an example label description.",
                ForeColor = foreColor,
                BackColor = backColor,
                SortOrder = sortOrder,
                CreatedUserId = user?.Id ?? 0,
                CreatedDate = DateTimeOffset.UtcNow
            });

            if (!categoryResult.Succeeded)
            {
                return result.Failed(result.Errors.ToArray());
            }

            return result.Success();

        }

        async Task<ICommandResultBase> UninstallLabelsAsync(IShellFeature feature)
        {

            // Validate

            if (feature == null)
            {
                throw new ArgumentNullException(nameof(feature));
            }

            if (string.IsNullOrEmpty(feature.ModuleId))
            {
                throw new ArgumentNullException(nameof(feature.ModuleId));
            }

            // Our result
            var result = new CommandResultBase();

            // Replacements for SQL script
            var replacements = new Dictionary<string, string>()
            {
                ["{featureId}"] = feature.Id.ToString()
            };

            // Sql to execute
            var sql = @"
                DELETE FROM {prefix}_EntityLabels WHERE LabelId IN (
                    SELECT Id FROM {prefix}_Labels WHERE FeatureId = {featureId}
                );
                DELETE FROM {prefix}_LabelData WHERE LabelId IN (
                    SELECT Id FROM {prefix}_Labels WHERE FeatureId = {featureId}
                );             
                DELETE FROM {prefix}_Labels WHERE FeatureId = {featureId};
            ";

            // Execute and return result
            var error = string.Empty;
            try
            {
                await _dbHelper.ExecuteScalarAsync<int>(sql, replacements);
            }
            catch (Exception e)
            {
                error = e.Message;
            }

            return !string.IsNullOrEmpty(error)
                ? result.Failed(error)
                : result.Success();

        }

    }

}
