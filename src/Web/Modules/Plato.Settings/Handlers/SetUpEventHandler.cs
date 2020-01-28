using System;
using System.Threading.Tasks;
using PlatoCore.Abstractions.Routing;
using PlatoCore.Abstractions.Settings;
using PlatoCore.Abstractions.SetUp;
using PlatoCore.Security.Abstractions;
using PlatoCore.Stores.Abstractions.Settings;
using PlatoCore.Text.Abstractions;

namespace Plato.Settings.Handlers
{
    public class SetUpEventHandler : BaseSetUpEventHandler
    {

        private readonly IDefaultRolesManager _defaultRolesManager;
        private readonly ISiteSettingsStore _siteSettingsStore;
        private readonly IKeyGenerator _keyGenerator;

        public SetUpEventHandler(
            IDefaultRolesManager defaultRolesManager,
            ISiteSettingsStore siteSettingsService, 
            IKeyGenerator keyGenerator)
        {
            _defaultRolesManager = defaultRolesManager;
            _siteSettingsStore = siteSettingsService;
            _keyGenerator = keyGenerator;
        }

        public override async Task SetUp(
            SetUpContext context,
            Action<string, string> reportError)
        {

            // --------------------------
            // Add default settings to dictionary store
            // --------------------------

            try
            {

                var siteSettings = await _siteSettingsStore.GetAsync() ?? new SiteSettings();
                siteSettings.SiteName = context.SiteName;
                siteSettings.SuperUser = context.AdminUsername;
                siteSettings.ApiKey = _keyGenerator.GenerateKey();
                siteSettings.HomeRoute = new HomeRoute();
                await _siteSettingsStore.SaveAsync(siteSettings);
            }
            catch (Exception ex)
            {
                reportError(ex.Message, ex.StackTrace);
            }

            // --------------------------
            // Apply default permissions to default roles for new feature
            // --------------------------

            await _defaultRolesManager.UpdateDefaultRolesAsync(new Permissions());

        }

    }

}
