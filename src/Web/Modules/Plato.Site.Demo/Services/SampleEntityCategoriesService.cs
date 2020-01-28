using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Plato.Categories.Models;
using Plato.Categories.Services;
using Plato.Categories.Stores;
using Plato.Entities.Models;
using Plato.Entities.Services;
using Plato.Entities.Stores;
using PlatoCore.Abstractions;
using PlatoCore.Data.Abstractions;
using PlatoCore.Features.Abstractions;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Models.Features;
using Plato.Site.Demo.Models;

namespace Plato.Site.Demo.Services
{

    public class SampleEntityCategoriesService : ISampleEntityCategoriesService
    {

        private readonly List<SampleDataDescriptor> Descriptors = new List<SampleDataDescriptor>()
        {
            new SampleDataDescriptor()
            {
                ModuleId = "Plato.Discuss.Categories",
                EntityType = "topic"
            },
            new SampleDataDescriptor()
            {
                ModuleId = "Plato.Docs.Categories",
                EntityType = "doc",
            },
            new SampleDataDescriptor()
            {
                ModuleId = "Plato.Articles.Categories",
                EntityType = "articles",
            },
            new SampleDataDescriptor()
            {
                ModuleId = "Plato.Ideas.Categories",
                EntityType = "idea"
            },
            new SampleDataDescriptor()
            {
                ModuleId = "Plato.Issues.Categories",
                EntityType = "issue"
            },
            new SampleDataDescriptor()
            {
                ModuleId = "Plato.Questions.Categories",
                EntityType = "question"
            }
        };

        private readonly Random _random;

        private readonly IEntityCategoryManager _entityCategoryManager;
        private readonly ICategoryStore<CategoryBase> _categoryStore;
        private readonly IEntityManager<Entity> _entityManager;
        private readonly IEntityStore<Entity> _entityStore;
        private readonly IContextFacade _contextFacade;
        private readonly IFeatureFacade _featureFacade;        
        private readonly IDbHelper _dbHelper;

        public SampleEntityCategoriesService(
            IEntityCategoryManager entityCategoryManager,
            ICategoryStore<CategoryBase> categoryStore,
            IEntityManager<Entity> entityManager,
            IEntityStore<Entity> entityStore,            
            IContextFacade contextFacade,
            IFeatureFacade featureFacade,
            IDbHelper dbHelper)
        {
            _entityCategoryManager = entityCategoryManager;
            _featureFacade = featureFacade;
            _contextFacade = contextFacade;            
            _entityManager = entityManager;
            _categoryStore = categoryStore;
            _entityStore = entityStore;
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

            // Get all feature tags
            var categories = await _categoryStore.QueryAsync()
              .Select<CategoryQueryParams>(Q =>
              {
                  Q.FeatureId.Equals(feature.Id);

              })
              .ToList();

            // Associate every category with at least 1 entity

            var output = new CommandResultBase();

            if (categories != null)
            {

                var entityFeature = await GetEntityFeatureAsync(feature);
                if (entityFeature == null)
                {
                    return output.Failed($"A feature named {feature.ModuleId.Replace(".Categories", "")} is not enabled!");
                }

                // Get entities for feature
                var entities = await _entityStore.QueryAsync()
                    .Select<EntityQueryParams>(q =>
                    {
                        q.FeatureId.Equals(entityFeature.Id);
                        q.CategoryId.Equals(0);
                    })
                    .ToList();

                // Keeps track of entities already added to categories
                var alreadyAdded = new Dictionary<int, Entity>();

                // Interate categories building random entities
                // not already added to a category and adding
                // those random entities to the current category
                foreach (var category in categories?.Data)
                {

                    // Get random entities 
                    var randomEntities = GetRandomEntities(entities?.Data, alreadyAdded);

                    // Ensure we have some random entities, they may have already al been added
                    if (randomEntities == null)
                    {
                        return output.Success();
                    }

                    // Add random entities to category
                    foreach (var randomEntity in randomEntities)
                    {

                        // Get the full entity
                        var entity = await _entityStore.GetByIdAsync(randomEntity.Id);

                        // Update
                        entity.CategoryId = category.Id;
                        entity.ModifiedUserId = user?.Id ?? 0;
                        entity.ModifiedDate =  DateTime.UtcNow;

                        // Persist
                        var entityResult = await _entityManager.UpdateAsync(entity);
                        if (entityResult.Succeeded)
                        {
                            // Add entity / category relationship
                            var result = await _entityCategoryManager.CreateAsync(new EntityCategory()
                            {
                                EntityId = entityResult.Response.Id,
                                CategoryId = category.Id,
                                CreatedUserId = user?.Id ?? 0,
                                CreatedDate = DateTime.UtcNow
                            });
                            if (!result.Succeeded)
                            {
                                return output.Failed(result.Errors.ToArray());
                            }
                        } else
                        {
                            return output.Failed(entityResult.Errors.ToArray());
                        }

                    }

                }

            }

            return output.Success();

        }

        async Task<IShellFeature> GetEntityFeatureAsync(IShellFeature feature)
        {
            var featureId = feature.ModuleId.Replace(".Categories", "");
            return await _featureFacade.GetFeatureByIdAsync(featureId);          
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

            // Get entities feature for categories feature
            var entityFeature = await GetEntityFeatureAsync(feature);
            if (entityFeature == null)
            {
                return result.Failed($"A feature named {feature.ModuleId.Replace(".Categories", "")} is not enabled!");
            }

            // Replacements for SQL script
            var replacements = new Dictionary<string, string>()
            {
                ["{featureId}"] = feature.Id.ToString(),
                ["{entityFeatureId}"] = entityFeature.Id.ToString()
            };

            // Sql to execute
            var sql = @"
                DELETE FROM {prefix}_EntityCategories WHERE CategoryId IN (
                    SELECT Id FROM {prefix}_Categories WHERE FeatureId = {featureId}
                );
                DELETE FROM {prefix}_CategoryData WHERE CategoryId IN (
                    SELECT Id FROM {prefix}_Categories WHERE FeatureId = {featureId}
                );
                UPDATE {prefix}_Entities SET CategoryId = 0 WHERE FeatureId = {entityFeatureId}
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
