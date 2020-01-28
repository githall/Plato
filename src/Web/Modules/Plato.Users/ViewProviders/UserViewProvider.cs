using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using PlatoCore.FileSystem.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Models.Shell;
using PlatoCore.Models.Users;
using PlatoCore.Stores.Abstractions.Files;
using PlatoCore.Stores.Abstractions.Users;
using Plato.Users.ViewModels;

namespace Plato.Users.ViewProviders
{
    public class UserViewProvider : BaseViewProvider<ProfilePage>
    {
  
        private readonly UserManager<User> _userManager;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IUserPhotoStore<UserPhoto> _userPhotoStore;
        private readonly ISitesFolder _sitesFolder;

        public UserViewProvider(
            IShellSettings shellSettings,
            IPlatoUserStore<User> platoUserStore,
            UserManager<User> userManager,
            IUserPhotoStore<UserPhoto> userPhotoStore,
            ISitesFolder sitesFolder,
            IHostEnvironment hostEnvironment,
            IFileStore fileStore)
        {
            _platoUserStore = platoUserStore;
            _userManager = userManager;
            _userPhotoStore = userPhotoStore;
            _sitesFolder = sitesFolder;
        }

        public override Task<IViewProviderResult> BuildIndexAsync(ProfilePage user, IViewProviderContext context)
        {

            var viewModel = context.Controller.HttpContext.Items[typeof(UserIndexViewModel)] as UserIndexViewModel;
            if (viewModel == null)
            {
                throw new Exception($"A view model of type {typeof(UserIndexViewModel).ToString()} has not been registered on the HttpContext!");
            }

            return Task.FromResult(
                Views(
                    View<UserIndexViewModel>("Home.Index.Header", model => viewModel).Zone("header"),
                    View<UserIndexViewModel>("Home.Index.Tools", model => viewModel).Zone("tools"),
                    View<UserIndexViewModel>("Home.Index.Content", model => viewModel).Zone("content")
                ));

        }
        
        public override async Task<IViewProviderResult> BuildDisplayAsync(ProfilePage profile, IViewProviderContext context)
        {

            var user = await _platoUserStore.GetByIdAsync(profile.Id);
            if (user == null)
            {
                return await BuildIndexAsync(profile, context);
            }

            return Views(
                View<User>("Home.Display.Header", model => user).Zone("header"),
                View<User>("Home.Display.Tools", model => user).Zone("tools"),
                View<User>("Home.Display.Content", model => user).Zone("content").Order(int.MinValue + 100),
                View<User>("Home.Display.Sidebar", model => user).Zone("sidebar")                
            );
        }

        public override Task<IViewProviderResult> BuildEditAsync(ProfilePage profile, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override Task<IViewProviderResult> BuildUpdateAsync(ProfilePage profile, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
    }

}
