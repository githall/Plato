﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Plato.Issues.Models;
using Plato.Entities.Stores;
using Plato.Follows.Services;
using Plato.Follows.Stores;
using Plato.Follows.ViewModels;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Security.Abstractions;
using PlatoCore.Hosting.Web.Abstractions;

namespace Plato.Issues.Follow.ViewProviders
{
    public class IssueViewProvider : ViewProviderBase<Issue>
    {

        private const string FollowHtmlName = "follow";
        
        private readonly IFollowStore<Plato.Follows.Models.Follow> _followStore;
        private readonly IFollowManager<Follows.Models.Follow> _followManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly IEntityStore<Issue> _entityStore;
        private readonly IContextFacade _contextFacade;
        private readonly HttpRequest _request;
 
        public IssueViewProvider(
            IFollowManager<Plato.Follows.Models.Follow> followManager,
            IFollowStore<Plato.Follows.Models.Follow> followStore,
            IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor,
            IEntityStore<Issue> entityStore,
            IContextFacade contextFacade)
        {
            _request = httpContextAccessor.HttpContext.Request;
            _authorizationService = authorizationService;
            _followManager = followManager;
            _contextFacade = contextFacade;
            _followStore = followStore;
            _entityStore = entityStore;
        }
        
        public override Task<IViewProviderResult> BuildIndexAsync(Issue entity, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override async Task<IViewProviderResult> BuildDisplayAsync(Issue entity, IViewProviderContext updater)
        {

            if (entity == null)
            {
                return await BuildIndexAsync(new Issue(), updater);
            }

            var isFollowing = false;
            var followType = FollowTypes.Issue;

            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (user != null)
            {
                var entityFollow = await _followStore.SelectByNameThingIdAndCreatedUserId(
                    followType.Name,
                    entity.Id,
                    user.Id);
                if (entityFollow != null)
                {
                    isFollowing = true;
                }
            }
            
            return Views(
                View<FollowViewModel>("Follow.Display.Tools", model =>
                {
                    model.FollowType = followType;
                    model.ThingId = entity.Id;
                    model.IsFollowing = isFollowing;
                    model.Permission = Permissions.FollowIssues;
                    return model;
                }).Zone("header-right").Order(-4)
            );

        }

        public override async Task<IViewProviderResult> BuildEditAsync(Issue entity, IViewProviderContext context)
        {
            if (entity == null)
            {
                return await BuildIndexAsync(new Issue(), context);
            }


            var isFollowing = false;
            var followType = FollowTypes.Issue;
            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (user != null)
            {
                var entityFollow = await _followStore.SelectByNameThingIdAndCreatedUserId(
                    followType.Name,
                    entity.Id,
                    user.Id);
                if (entityFollow != null)
                {
                    isFollowing = true;
                }
            }
            
            // For new entities check if we need to follow by default
            if (entity.Id == 0)
            {
                if (await _authorizationService.AuthorizeAsync(context.Controller.HttpContext.User,
                    entity.CategoryId, Permissions.AutoFollowIssues))
                {
                    isFollowing = true;
                }
            }

            return Views(
                View<FollowViewModel>("Follow.Edit.Sidebar", model =>
                {
                    model.FollowType = followType;
                    model.FollowHtmlName = FollowHtmlName;
                    model.ThingId = entity.Id;
                    model.IsFollowing = isFollowing;
                    model.Permission = Follow.Permissions.FollowIssues;
                    return model;
                }).Zone("content-right").Order(20)
            );

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(Issue issue, IViewProviderContext updater)
        {

            // Ensure entity exists before attempting to update
            var entity = await _entityStore.GetByIdAsync(issue.Id);
            if (entity == null)
            {
                return await BuildEditAsync(issue, updater);
            }

            // Get the follow checkbox value
            var follow = false;
            foreach (var key in _request.Form.Keys)
            {
                if (key == FollowHtmlName)
                {
                    var values = _request.Form[key];
                    if (!String.IsNullOrEmpty(values))
                    {
                        follow = true;
                        break;
                    }
                }
            }

            // We need to be authenticated to follow
            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return await BuildEditAsync(issue, updater);
            }

            // The follow type
            var followType = FollowTypes.Issue;
      
            // Get any existing follow
            var existingFollow = await _followStore.SelectByNameThingIdAndCreatedUserId(
                followType.Name,
                entity.Id,
                user.Id);
            
            // Add the follow
            if (follow)
            {
                // If we didn't find an existing follow create a new one
                if (existingFollow == null)
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
      
            }
            else
            {
                if (existingFollow != null)
                {
                    await _followManager.DeleteAsync(existingFollow);
                }
            }

            return await BuildEditAsync(issue, updater);

        }

    }

}
