using System.Collections.Generic;
using System.Threading.Tasks;
using PlatoCore.Cache.Abstractions;
using PlatoCore.Models.Schema;
using PlatoCore.Repositories.Schema;
using PlatoCore.Stores.Abstractions.Schema;

namespace PlatoCore.Stores.Schema
{
    
    public class ConstraintStore : IConstraintStore
    {

        private readonly ICacheManager _cacheManager;
        private readonly IConstraintRepository _constraintRepository;

        public ConstraintStore(
            ICacheManager cacheManager,
            IConstraintRepository constraintRepository)
        {
            _cacheManager = cacheManager;
            _constraintRepository = constraintRepository;
        }

        public async Task<IEnumerable<DbConstraint>> SelectConstraintsAsync()
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType());
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _constraintRepository.SelectConstraintsAsync());
        }

    }

}
