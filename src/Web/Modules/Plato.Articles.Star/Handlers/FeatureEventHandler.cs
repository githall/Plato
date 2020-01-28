using System.Threading.Tasks;
using PlatoCore.Features.Abstractions;
using PlatoCore.Security.Abstractions;

namespace Plato.Articles.Star.Handlers
{
    
    public class FeatureEventHandler : BaseFeatureEventHandler
    {
        
        private readonly IDefaultRolesManager _defaultRolesManager;

        public FeatureEventHandler(IDefaultRolesManager defaultRolesManager)
        {
            _defaultRolesManager = defaultRolesManager;
        }
        
        public override async Task InstalledAsync(IFeatureEventContext context)
        {
            // Apply default permissions to default roles for new feature
            await _defaultRolesManager.UpdateDefaultRolesAsync(new Permissions());
        }

        public override async Task UpdatedAsync(IFeatureEventContext context)
        {
            // Apply any additional permissions to default roles for updated feature
            await _defaultRolesManager.UpdateDefaultRolesAsync(new Permissions());
        }

    }

}
