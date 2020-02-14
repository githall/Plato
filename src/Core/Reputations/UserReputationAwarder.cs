using System;
using System.Linq;
using System.Threading.Tasks;
using PlatoCore.Features.Abstractions;
using PlatoCore.Models.Reputations;
using PlatoCore.Reputations.Abstractions;

namespace PlatoCore.Reputations
{

    public class UserReputationAwarder : IUserReputationAwarder
    {

        private readonly IReputationsManager<Reputation> _reputationManager;
        private readonly IUserReputationManager<UserReputation> _userReputationManager;
        private readonly IFeatureFacade _featureFacade;

        public UserReputationAwarder(
            IUserReputationManager<UserReputation> userReputationManager,
            IReputationsManager<Reputation> reputationManager,
            IFeatureFacade featureFacade)
        {
            _userReputationManager = userReputationManager;
            _reputationManager = reputationManager;
            _featureFacade = featureFacade;
        }

        public async Task<UserReputation> AwardAsync(
            IReputation reputation, 
            int userId, 
            string description)
        {

            if (reputation == null)
            {
                throw new ArgumentNullException(nameof(reputation));
            }

            if (userId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(userId));
            }

            var userReputation = new UserReputation()
            {
                FeatureId = await GetReputationFeatureIdAsync(reputation),
                Name = reputation.Name,
                Description = description,
                Points = reputation.Points,
                CreatedUserId = userId
            };

            var result = await _userReputationManager.CreateAsync(userReputation);
            if (result.Succeeded)
            {
                return result.Response;
            }

            return null;

        }

        public async Task<UserReputation> RevokeAsync(
            IReputation reputation,
            int userId, 
            string description)
        {

            if (reputation == null)
            {
                throw new ArgumentNullException(nameof(reputation));
            }

            if (userId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(userId));
            }

            var userReputation = new UserReputation()
            {
                FeatureId = await GetReputationFeatureIdAsync(reputation),
                Name = reputation.Name,
                Description = description,
                Points = -reputation.Points,
                CreatedUserId = userId
            };

            var result = await _userReputationManager.CreateAsync(userReputation);
            if (result.Succeeded)
            {
                return result.Response;
            }

            return null;

        }

        // ------------

        async Task<int> GetReputationFeatureIdAsync(IReputation reputation)
        {

            var reputations = await _reputationManager.GetReputationsAsync();
            var providedReputation = reputations.FirstOrDefault(r => r.Name.Equals(reputation.Name));
            if (providedReputation != null)
            {
                if (!string.IsNullOrEmpty(providedReputation.ModuleId))
                {
                    var feature = await _featureFacade.GetFeatureByIdAsync(providedReputation.ModuleId);
                    if (feature != null)
                    {
                        return feature.Id;
                    }
                }
            }

            return 0;

        }

    }

}
