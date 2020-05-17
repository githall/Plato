﻿using System;
using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Entities.Services;
using Plato.Questions.Models;
using Plato.Entities.ViewModels;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Models.Users;
using PlatoCore.Stores.Abstractions.Users;

namespace Plato.Questions.ViewProviders
{

    public class UserViewProvider : ViewProviderBase<UserIndex>
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
        
        public override async Task<IViewProviderResult> BuildDisplayAsync(UserIndex userIndex, IViewProviderContext context)
        {

            // Get user
            var user = await _platoUserStore.GetByIdAsync(userIndex.Id);
            if (user == null)
            {
                return await BuildIndexAsync(userIndex, context);
            }

     
            // Get index view model from context
            var indexViewModel = context.Controller.HttpContext.Items[typeof(EntityIndexViewModel<Question>)] as EntityIndexViewModel<Question>;
            if (indexViewModel == null)
            {
                throw new Exception($"A view model of type {typeof(EntityIndexViewModel<Question>).ToString()} has not been registered on the HttpContext!");
            }

            // Build feature entities model
            var featureEntityMetrics = new FeatureEntityCounts()
            {
                Features = await _featureEntityCountService
                    .ConfigureQuery(q =>
                    {
                        q.CreatedUserId.Equals(user.Id);
                        q.HideSpam.True();
                        q.HideHidden.True();
                        q.HideDeleted.True();
                        q.HidePrivate.True();

                    })
                    .GetResultsAsync()
            };

            // Build view model
            var userDisplayViewModel = new UserDisplayViewModel<Question>()
            {
                User = user,
                IndexViewModel = indexViewModel,
                Counts = featureEntityMetrics
            };
            
            // Build view
            return Views(
                View<UserDisplayViewModel>("User.Index.Header", model => userDisplayViewModel).Zone("header"),
                View<UserDisplayViewModel<Question>>("User.Index.Content", model => userDisplayViewModel).Zone("content"),
                View<UserDisplayViewModel>("User.Entities.Display.Sidebar", model => userDisplayViewModel).Zone("content-right")
            );
            
        }

        public override Task<IViewProviderResult> BuildIndexAsync(UserIndex model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(UserIndex userIndex, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(UserIndex model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

    }

}
