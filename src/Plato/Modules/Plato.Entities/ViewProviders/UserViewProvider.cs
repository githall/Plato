using System;
using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Entities.Services;
using Plato.Entities.ViewModels;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Entities.ViewProviders
{

    public class UserViewProvider : BaseViewProvider<EntityUserIndex>
    {

        private readonly IFeatureEntityCountService _featureEntityCountService;        
        private readonly IPlatoUserStore<User> _platoUserStore;  

        public UserViewProvider(                        
            IFeatureEntityCountService featureEntityCountService,
            IPlatoUserStore<User> platoUserStore)
        {
            _featureEntityCountService = featureEntityCountService;
            _platoUserStore = platoUserStore;            
        }
        
        public override async Task<IViewProviderResult> BuildDisplayAsync(EntityUserIndex userIndex, IViewProviderContext context)
        {

            var user = await _platoUserStore.GetByIdAsync(userIndex.Id);
            if (user == null)
            {
                return await BuildIndexAsync(userIndex, context);
            }

            // Get view model from context
            var indexViewModel = context.Controller.HttpContext.Items[typeof(EntityIndexViewModel<Entity>)] as EntityIndexViewModel<Entity>;

            // We always need a view module
            if (indexViewModel == null)
            {
                throw new Exception($"A view model of type {typeof(EntityIndexViewModel<Entity>).ToString()} has not been registered on the HttpContext!");
            }

            var featureEntityMetrics = new FeatureEntityCounts()
            {
                Features = await _featureEntityCountService
                    .ConfigureQuery(q =>
                    {
                        // Display all public entities for the given user                        
                        q.CreatedUserId.Equals(user.Id);
                        q.HideSpam.True();
                        q.HideHidden.True();
                        q.HideDeleted.True();
                        q.HidePrivate.True();
                    })
                    .GetResultsAsync()
            };

            var userDisplayViewModel = new UserDisplayViewModel<Entity>()
            {
                User = user,
                IndexViewModel = indexViewModel,
                Counts = featureEntityMetrics
            };
            
            return Views(
                View<UserDisplayViewModel>("User.Index.Header", model => userDisplayViewModel).Zone("header"),
                View<UserDisplayViewModel<Entity>>("User.Index.Content", model => userDisplayViewModel).Zone("content"),
                View< UserDisplayViewModel>("User.Entities.Display.Sidebar", model => userDisplayViewModel).Zone("sidebar")
            );

        }

        public override Task<IViewProviderResult> BuildIndexAsync(EntityUserIndex model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(EntityUserIndex userIndex, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(EntityUserIndex model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

    }

}
