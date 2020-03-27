using System.Linq;
using System.Threading.Tasks;
using Plato.Docs.Models;
using Plato.Entities.Services;
using Plato.Entities.Stores;
using PlatoCore.Abstractions;
using PlatoCore.Features.Abstractions;

namespace Plato.Docs.Services
{

    public class DocManager : IPostManager<Doc>
    {

        private readonly ISimpleEntityStore<SimpleDoc> _simpleEntityStore;
        private readonly IEntityManager<Doc> _entityManager;
        private readonly IFeatureFacade _featureFacade;
        
        public DocManager(
            IEntityManager<Doc> entityManager,
            ISimpleEntityStore<SimpleDoc> simpleEntityStore,
            IFeatureFacade featureFacade)
        {
            _simpleEntityStore = simpleEntityStore; 
            _entityManager = entityManager;            
            _featureFacade = featureFacade;
        }

        public async Task<ICommandResult<Doc>> CreateAsync(Doc model)
        {

            if (model.FeatureId == 0)
            {
                var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Docs");
                if (feature != null)
                {
                    model.FeatureId = feature.Id;
                }
            }
            
            var result = await _entityManager.CreateAsync(model);

            if (result.Succeeded)
            {
                // Expire simple entity cache for docs
                _simpleEntityStore.CancelTokens(null);
            }

            return result;

        }

        public async Task<ICommandResult<Doc>> UpdateAsync(Doc model)
        {

            // We always need a feature
            if (model.FeatureId == 0)
            {
                var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Docs");
                if (feature != null)
                {
                    model.FeatureId = feature.Id;
                }
            }
            
            var result = await _entityManager.UpdateAsync(model);

            if (result.Succeeded)
            {
                // Expire simple entity cache for docs
                _simpleEntityStore.CancelTokens(null);
            }

            return null;

        }

        public async Task<ICommandResult<Doc>> DeleteAsync(Doc model)
        {

            var result = await _entityManager.DeleteAsync(model);

            if (result.Succeeded)
            {
                // Expire simple entity cache for docs
                _simpleEntityStore.CancelTokens(null);
            }

            return result;
        }

        public async Task<ICommandResult<Doc>> Move(Doc model, MoveDirection direction)
        {

            var result = await _entityManager.Move(model, direction);

            if (result.Succeeded)
            {
                // Expire simple entity cache for docs
                _simpleEntityStore.CancelTokens(null);
            }

            return result;

        }
        
    }

}
