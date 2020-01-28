using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Plato.Entities.Models;
using Plato.Entities.Services;
using Plato.Entities.ViewModels;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Models.Users;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Stores.Abstractions.Users;
using PlatoCore.Security.Abstractions;

namespace Plato.Entities.ViewProviders
{

    public class ProfileViewProvider : BaseViewProvider<ProfilePage>
    {
        
        private readonly IFeatureEntityCountService _featureEntityCountService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IPlatoUserStore<User> _platoUserStore;
        
        public ProfileViewProvider(
            IFeatureEntityCountService featureEntityCountService,
            IAuthorizationService authorizationService,
            IPlatoUserStore<User> platoUserStore)
        {
            _featureEntityCountService = featureEntityCountService;
            _authorizationService = authorizationService;
            _platoUserStore = platoUserStore;
        }

        public override async Task<IViewProviderResult> BuildDisplayAsync(ProfilePage profile, IViewProviderContext context)
        {

            // Get user
            var user = await _platoUserStore.GetByIdAsync(profile.Id);

            // Ensure user exists
            if (user == null)
            {
                return await BuildIndexAsync(profile, context);
            }
            
            var indexOptions = new EntityIndexOptions()
            {
                CreatedByUserId = user.Id
            };
            

            var featureEntityMetrics = new FeatureEntityCounts()
            {
                Features = await _featureEntityCountService
                    .ConfigureQuery(async q =>
                    {

                        // Hide private?
                        if (!await _authorizationService.AuthorizeAsync(context.Controller.HttpContext.User,
                            Permissions.ViewPrivateEntities))
                        {
                            q.HidePrivate.True();
                        }

                        // Hide hidden?
                        if (!await _authorizationService.AuthorizeAsync(context.Controller.HttpContext.User,
                            Permissions.ViewHiddenEntities))
                        {
                            q.HideHidden.True();
                        }

                        // Hide spam?
                        if (!await _authorizationService.AuthorizeAsync(context.Controller.HttpContext.User,
                            Permissions.ViewSpamEntities))
                        {
                            q.HideSpam.True();
                        }

                        // Hide deleted?
                        if (!await _authorizationService.AuthorizeAsync(context.Controller.HttpContext.User,
                            Permissions.ViewDeletedEntities))
                        {
                            q.HideDeleted.True();
                        }

                    })
                    .GetResultsAsync(indexOptions)
            };
            
            var viewModel = new UserDisplayViewModel<Entity>()
            {
                User = user,
                Counts = featureEntityMetrics,
                IndexViewModel = new EntityIndexViewModel<Entity>()
                {
                    Options = indexOptions,
                    Pager = new PagerOptions()
                    {
                        Page = 1,
                        Size = 10,
                        Enabled = false
                    }
                }
            };
         
            // Return view
            return Views(
                View<UserDisplayViewModel<Entity>>("Profile.Entities.Display.Content", model => viewModel)
                    .Zone("content")
                    .Order(int.MaxValue - 100)
            );

        }

        public override Task<IViewProviderResult> BuildIndexAsync(ProfilePage model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(ProfilePage model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(ProfilePage model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

    }

}
