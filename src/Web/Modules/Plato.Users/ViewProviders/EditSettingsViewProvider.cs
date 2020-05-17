using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using PlatoCore.Layout.ModelBinding;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Models.Users;
using PlatoCore.Stores.Abstractions.Users;
using Plato.Users.Services;
using Plato.Users.ViewModels;

namespace Plato.Users.ViewProviders
{

    public class EditSettingsViewProvider : ViewProviderBase<EditSettingsViewModel>
    {
        
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IPlatoUserManager<User> _platoUserManager;

        public EditSettingsViewProvider(
            IPlatoUserStore<User> platoUserStore,
            IPlatoUserManager<User> platoUserManager)
        {
            _platoUserStore = platoUserStore;
            _platoUserManager = platoUserManager;
        }

        #region "Implementation"

        public override Task<IViewProviderResult> BuildIndexAsync(EditSettingsViewModel viewModel, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildDisplayAsync(EditSettingsViewModel viewModel, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(EditSettingsViewModel viewModel, IViewProviderContext updater)
        {

            var user = await _platoUserStore.GetByIdAsync(viewModel.Id);
            if (user == null)
            {
                return await BuildIndexAsync(viewModel, updater);
            }

            return Views(
                View<User>("Home.Edit.Header", model => user).Zone("header"),                
                View<User>("Home.Edit.Tools", model => user).Zone("header-right"),
                View<User>("Home.Edit.Sidebar", model => user).Zone("content-left"),
                View<EditSettingsViewModel>("Home.EditSettings.Content", model => viewModel).Zone("content"),
                View<User>("Home.Edit.Footer", model => user).Zone("actions")
            );

        }

        public override async Task<bool> ValidateModelAsync(EditSettingsViewModel viewModel, IUpdateModel updater)
        {
            return await updater.TryUpdateModelAsync(viewModel);
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(EditSettingsViewModel viewModel, IViewProviderContext context)
        {
            var user = await _platoUserStore.GetByIdAsync(viewModel.Id);
            if (user == null)
            {
                return await BuildIndexAsync(viewModel, context);
            }

            var model = new EditSettingsViewModel();;

            if (!await context.Updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(viewModel, context);
            }

            return await BuildEditAsync(viewModel, context);

        }

        #endregion
      
    }

}
