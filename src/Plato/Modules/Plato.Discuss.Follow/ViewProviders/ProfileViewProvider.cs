﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Discuss.Models;
using Plato.Discuss.ViewModels;
using Plato.Follow;
using Plato.Follow.Stores;
using Plato.Follow.ViewModels;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Discuss.Follow.ViewProviders
{
    public class ProfileViewProvider : BaseViewProvider<UserProfile>
    {

        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IContextFacade _contextFacade;
        private readonly IFollowStore<Plato.Follow.Models.Follow> _followStore;

        public ProfileViewProvider(
            IPlatoUserStore<User> platoUserStore, 
            IContextFacade contextFacade,
            IFollowStore<Plato.Follow.Models.Follow> followStore)
        {
            _platoUserStore = platoUserStore;
            _contextFacade = contextFacade;
            _followStore = followStore;
        }

        public override async Task<IViewProviderResult> BuildDisplayAsync(UserProfile discussUser, IViewProviderContext context)
        {

            var user = await _platoUserStore.GetByIdAsync(discussUser.Id);
            if (user == null)
            {
                return await BuildIndexAsync(discussUser, context);
            }

            var followType = DefaultFollowTypes.User;
            var isFollowing = false;

            var currentUser = await _contextFacade.GetAuthenticatedUserAsync();
            if (currentUser != null)
            {
                var entityFollow = await _followStore.SelectFollowByNameThingIdAndCreatedUserId(
                    followType.Name,
                    user.Id,
                    currentUser.Id);
                if (entityFollow != null)
                {
                    isFollowing = true;
                }
            }
            
            return Views(
                View<FollowViewModel>("Follow.Display.Tools", model =>
                {
                    model.FollowType = followType;
                    model.EntityId = user.Id;
                    model.IsFollowing = isFollowing;
                    return model;
                }).Zone("tools").Order(1)
            );


        }

        public override Task<IViewProviderResult> BuildIndexAsync(UserProfile model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(UserProfile discussUser, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(UserProfile model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
    }

}