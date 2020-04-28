using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using PlatoCore.Abstractions;
using PlatoCore.Data.Abstractions;
using PlatoCore.Features.Abstractions;
using PlatoCore.Models.Features;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using Plato.Site.Demo.Models;
using Plato.Tags.Models;
using Plato.Tags.Services;
using Plato.Tags.Stores;
using PlatoCore.Hosting.Web.Abstractions;

namespace Plato.Site.Demo.Services
{

    public class SampleEntityTagsService : ISampleEntityTagsService
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

        private readonly IEntityTagManager<EntityTag> _entityTagManager;
        private readonly IEntityStore<Entity> _entityStore;        
        private readonly IContextFacade _contextFacade;
        private readonly IFeatureFacade _featureFacade;
        private readonly ITagStore<TagBase> _tagStore;
        private readonly IDbHelper _dbHelper;

        public SampleEntityTagsService(
            IEntityTagManager<EntityTag> entityTagManager,
            IEntityStore<Entity> entityStore,
            ITagStore<TagBase> tagStore,
            IContextFacade contextFacade,
            IFeatureFacade featureFacade,
            IDbHelper dbHelper)
        {
            _entityTagManager = entityTagManager;
            _featureFacade = featureFacade;
            _contextFacade = contextFacade;
            _entityStore = entityStore;
            _tagStore = tagStore;
            _dbHelper = dbHelper;
            _random = new Random();
        }

        public async Task<ICommandResultBase> InstallAsync()
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

                var result = await InstallInternalAsync(feature);
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

                var result = await UninstallInternalAsync(feature);
                if (!result.Succeeded)
                {
                    return output.Failed(result.Errors.ToArray());
                }

            }

            return output.Success();

        }

        // ----------

        async Task<ICommandResultBase> InstallInternalAsync(IShellFeature feature)
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

            // Get all tags
            var tags = await _tagStore.QueryAsync()
              .Select<TagQueryParams>(Q =>
              {
                  Q.FeatureId.Equals(feature.Id);

              })
              .ToList();

            // Associate every tag with at least 1 entity

            var output = new CommandResultBase();

            if (tags != null)
            {

                // Get all entities
                var entities = await _entityStore.QueryAsync()
                    .Select<EntityQueryParams>(q =>
                    {
                        q.FeatureId.Equals(feature.Id);
                    })
                    .ToList();

                var alreadyAdded = new Dictionary<int, Entity>();
                foreach (var tag in tags?.Data)
                {

                    var randomEntities = GetRandomEntities(entities?.Data, alreadyAdded);

                    if (randomEntities == null)
                    {
                        return output.Success();
                    }

                    foreach (var randomEntity in randomEntities)
                    {
                        var result = await _entityTagManager.CreateAsync(new EntityTag()
                        {
                            EntityId = randomEntity.Id,
                            TagId = tag.Id,
                            CreatedUserId = user?.Id ?? 0,
                            CreatedDate = DateTime.UtcNow
                        });

                        if (!result.Succeeded)
                        {
                            return output.Failed(result.Errors.ToArray());
                        }
                    }
                }
            }

            return output.Success();

        }

        async Task<ICommandResultBase> UninstallInternalAsync(IShellFeature feature)
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
                DELETE FROM {prefix}_EntityTags WHERE TagId IN (
                    SELECT Id FROM {prefix}_Tags WHERE FeatureId = {featureId}
                );
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

        IList<Entity> GetRandomEntities(IList<Entity> entities, IDictionary<int, Entity> alreadyAdded)
        {

            if (entities == null)
            {
                return null;
            }

            var output = new Dictionary<int, Entity>();
            for (var i = 0; i < _random.Next(1, 4); i++)
            {
                var index = _random.Next(0, entities.Count - 1);
                var entity = entities[index];
                if (!output.ContainsKey(entity.Id) && !alreadyAdded.ContainsKey(entity.Id))
                {
                    output.Add(entity.Id, entity);
                    alreadyAdded.Add(entity.Id, entity);
                }
            }

            return output.Values.ToList();

        }

    }

}
