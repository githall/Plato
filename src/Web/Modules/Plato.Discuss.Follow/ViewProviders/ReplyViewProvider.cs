﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Plato.Discuss.Models;
using Plato.Entities.Stores;
using Plato.Follows.Services;
using Plato.Follows.Stores;
using PlatoCore.Hosting.Web.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Security.Abstractions;

namespace Plato.Discuss.Follow.ViewProviders
{

    public class ReplyViewProvider : ViewProviderBase<Reply>
    {

        private readonly IFollowStore<Plato.Follows.Models.Follow> _followStore;
        private readonly IFollowManager<Follows.Models.Follow> _followManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly IEntityStore<Topic> _entityStore;
        private readonly IContextFacade _contextFacade;

        public ReplyViewProvider(
            IFollowManager<Follows.Models.Follow> followManager,
            IFollowStore<Follows.Models.Follow> followStore,
            IAuthorizationService authorizationService,
            IEntityStore<Topic> entityStore,
            IContextFacade contextFacade)
        {
            _authorizationService = authorizationService;
            _followManager = followManager;
            _contextFacade = contextFacade;
            _entityStore = entityStore;
            _followStore = followStore;
        }
        
        public override Task<IViewProviderResult> BuildDisplayAsync(Reply model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildIndexAsync(Reply model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(Reply reply, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(Reply reply, IViewProviderContext context)
        {

            if (reply == null)
            {
                return await BuildIndexAsync(new Reply(), context);
            }
            
            // Get authenticated user
            var user = await _contextFacade.GetAuthenticatedUserAsync(context.Controller.HttpContext.User?.Identity);

            // We must be authenticated to automatically follow entities
            if (user == null)
            {
                return await BuildEditAsync(reply, context);
            }

            // Get entity for reply
            var entity = await _entityStore.GetByIdAsync(reply.EntityId);

            // We always need an entity
            if (entity == null)
            {
                return await BuildEditAsync(reply, context);
            }
            
            // Are we authorized to automatically follow entities we participate in?
            if (!await _authorizationService.AuthorizeAsync(context.Controller.HttpContext.User,
                entity.CategoryId, Permissions.AutoFollowTopicReplies))
            {
                return await BuildEditAsync(reply, context);
            }

            // ---------
            // Automatically follow entities upon a reply
            // ---------

            var followType = FollowTypes.Topic;
            
            // Are we already following the entity?
            var entityFollow = await _followStore.SelectByNameThingIdAndCreatedUserId(
                followType.Name,
                entity.Id,
                user.Id);

            // Ensure we are not already following the entity
            if (entityFollow == null)
            {
                // Add follow
                await _followManager.CreateAsync(new Follows.Models.Follow()
                {
                    Name = followType.Name,
                    ThingId = entity.Id,
                    CreatedUserId = user.Id,
                    CreatedDate = DateTime.UtcNow
                });
            }

            return await BuildEditAsync(reply, context);

        }

    }

}
