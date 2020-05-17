﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Plato.Articles.Follow.NotificationTypes;
using Plato.Articles.Models;
using Plato.Entities.Stores;
using Plato.Follows.Services;
using Plato.Follows.Stores;
using Plato.Follows.ViewModels;
using PlatoCore.Hosting.Web.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Security.Abstractions;
using Plato.Articles.Follow.ViewModels;
using Plato.Entities.Extensions;
using Plato.Entities.Models;
using PlatoCore.Models.Notifications;
using PlatoCore.Models.Users;
using PlatoCore.Notifications.Abstractions;
using PlatoCore.Notifications.Extensions;
using PlatoCore.Stores.Abstractions.Users;
using PlatoCore.Stores.Users;
using PlatoCore.Tasks.Abstractions;

namespace Plato.Articles.Follow.ViewProviders
{
    public class ArticleViewProvider : ViewProviderBase<Article>
    {

        private const string FollowHtmlName = "follow";
        private const string NotifyHtmlName = "notify";

        private readonly IUserNotificationTypeDefaults _userNotificationTypeDefaults;
        private readonly IDummyClaimsPrincipalFactory<User> _claimsPrincipalFactory;
        private readonly INotificationManager<Article> _notificationManager;
        private readonly IFollowStore<Plato.Follows.Models.Follow> _followStore;
        private readonly IFollowManager<Follows.Models.Follow> _followManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly IDeferredTaskManager _deferredTaskManager;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IEntityStore<Article> _entityStore;
        private readonly IContextFacade _contextFacade;
        private readonly HttpRequest _request;
 
        public ArticleViewProvider(
            IUserNotificationTypeDefaults userNotificationTypeDefaults,
            IDummyClaimsPrincipalFactory<User> claimsPrincipalFactory,
            IFollowManager<Plato.Follows.Models.Follow> followManager,
            IFollowStore<Plato.Follows.Models.Follow> followStore,
            INotificationManager<Article> notificationManager,
            IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor,
            IDeferredTaskManager deferredTaskManager,
            IPlatoUserStore<User> platoUserStore,
            IEntityStore<Article> entityStore,
            IContextFacade contextFacade)
        {
            _userNotificationTypeDefaults = userNotificationTypeDefaults;
            _request = httpContextAccessor.HttpContext.Request;
            _claimsPrincipalFactory = claimsPrincipalFactory;
            _authorizationService = authorizationService;
            _notificationManager = notificationManager;
            _deferredTaskManager = deferredTaskManager;
            _platoUserStore = platoUserStore;
            _followManager = followManager;
            _contextFacade = contextFacade;
            _followStore = followStore;
            _entityStore = entityStore;
        }
        
        public override Task<IViewProviderResult> BuildIndexAsync(Article entity, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override async Task<IViewProviderResult> BuildDisplayAsync(Article entity, IViewProviderContext updater)
        {

            if (entity == null)
            {
                return await BuildIndexAsync(new Article(), updater);
            }

            var isFollowing = false;
            var followType = FollowTypes.Article;

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
                    model.Permission = Permissions.FollowArticles;
                    return model;
                }).Zone("header-right").Order(-4)
            );

        }

        public override async Task<IViewProviderResult> BuildEditAsync(Article entity, IViewProviderContext context)
        {
            if (entity == null)
            {
                return await BuildIndexAsync(new Article(), context);
            }


            var isFollowing = false;
            var followType = FollowTypes.Article;
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
                    entity.CategoryId, Permissions.AutoFollowArticles))
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
                    model.Permission = Permissions.FollowArticles;
                    return model;
                }).Zone("content-right").Order(20),
                 View<EditFooterViewModel>("Article.Follow.Edit.Footer", model =>
                 {
                     model.IsNewEntity = entity.Id == 0 ? true : false;
                     model.NotifyHtmlName = NotifyHtmlName;
                     model.Permission = Permissions.SendArticleFollows;
                     return model;
                 }).Zone("actions")
            );

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(Article article, IViewProviderContext updater)
        {

            // Ensure entity exists before attempting to update
            var entity = await _entityStore.GetByIdAsync(article.Id);
            if (entity == null)
            {
                return await BuildEditAsync(article, updater);
            }

            // We need to be authenticated to follow
            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return await BuildEditAsync(article, updater);
            }

            // -----------------
            // Update follow status
            // -----------------

            await UpdateFollowStatus(entity, user);

            // -----------------
            // Send update notifications?
            // -----------------

            if (NotifyPostedValue())
            {
                // Exclude the user who modified the entity
                var usersToExclude = new List<int>()
                {
                    entity.ModifiedUserId
                };
                await SendNotificationsAsync(entity, usersToExclude);
            }

            return await BuildEditAsync(article, updater);

        }

        async Task UpdateFollowStatus(IEntity entity, IUser user)
        {

            // The follow type
            var followType = FollowTypes.Article;

            // Get any existing follow
            var existingFollow = await _followStore.SelectByNameThingIdAndCreatedUserId(
                followType.Name,
                entity.Id,
                user.Id);

            // Add the follow
            if (FollowPostedValue())
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

        }
        
        Task<Article> SendNotificationsAsync(Article entity, IList<int> usersToExclude)
        {

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            
            // Add deferred task
            _deferredTaskManager.AddTask(async context =>
            {

                // Follow type name
                var name = FollowTypes.Article.Name;

                // Get all follows for entity
                var follows = await _followStore.QueryAsync()
                    .Select<FollowQueryParams>(q =>
                    {
                        q.ThingId.Equals(entity.Id);
                        q.Name.Equals(name);
                        if (usersToExclude.Count > 0)
                        {
                            q.CreatedUserId.IsNotIn(usersToExclude.ToArray());
                        }
                    })
                    .ToList();

                // No follows simply return
                if (follows?.Data == null)
                {
                    return;
                }

                // Get users
                var users = await ReduceUsersAsync(follows?.Data, entity);

                // We always need users
                if (users == null)
                {
                    return;
                }

                // Send notifications
                foreach (var user in users)
                {

                    // Email notifications
                    if (user.NotificationEnabled(_userNotificationTypeDefaults, EmailNotifications.EntityUpdate))
                    {
                        await _notificationManager.SendAsync(new Notification(EmailNotifications.EntityUpdate)
                        {
                            To = user,
                        }, entity);
                    }

                    // Web notifications
                    if (user.NotificationEnabled(_userNotificationTypeDefaults, WebNotifications.EntityUpdate))
                    {
                        await _notificationManager.SendAsync(new Notification(WebNotifications.EntityUpdate)
                        {
                            To = user,
                            From = new User()
                            {
                                Id = entity.ModifiedBy.Id,
                                UserName = entity.ModifiedBy.UserName,
                                DisplayName = entity.ModifiedBy.DisplayName,
                                Alias = entity.ModifiedBy.Alias,
                                PhotoUrl = entity.ModifiedBy.PhotoUrl,
                                PhotoColor = entity.ModifiedBy.PhotoColor
                            }
                        }, entity);
                    }

                }
                
            });

            return Task.FromResult(entity);

        }

        async Task<IEnumerable<IUser>> ReduceUsersAsync(IEnumerable<Follows.Models.Follow> follows, IEntity entity)
        {

            // We always need follows to process
            if (follows == null)
            {
                return null;
            }
            
            // Get all users following the entity
            var users = await _platoUserStore.QueryAsync()
                .Select<UserQueryParams>(q =>
                {
                    q.Id.IsIn(follows
                        .Select(f => f.CreatedUserId)
                        .ToArray());
                })
                .ToList();

            // No users to further process
            if (users?.Data == null)
            {
                return null;
            }

            // Build users reducing for permissions
            var result = new Dictionary<int, IUser>();
            foreach (var user in users.Data)
            {

                if (!result.ContainsKey(user.Id))
                {
                    result.Add(user.Id, user);
                }

                // If the entity is hidden but the user does
                // not have permission to view hidden entities
                if (entity.IsHidden)
                {
                    var principal = await _claimsPrincipalFactory.CreateAsync(user);
                    if (!await _authorizationService.AuthorizeAsync(principal,
                        entity.CategoryId, Articles.Permissions.ViewHiddenArticles))
                    {
                        result.Remove(user.Id);
                    }
                }

                // If we are not the entity author and the entity is private
                // ensure we have permission to view private entities
                if (user.Id != entity.CreatedUserId && entity.IsPrivate)
                {
                    var principal = await _claimsPrincipalFactory.CreateAsync(user);
                    if (!await _authorizationService.AuthorizeAsync(principal,
                        entity.CategoryId, Articles.Permissions.ViewPrivateArticles))
                    {
                        result.Remove(user.Id);
                    }
                }

                // The entity has been flagged as SPAM but the user does
                // not have permission to view entities flagged as SPAM
                if (entity.IsSpam)
                {
                    var principal = await _claimsPrincipalFactory.CreateAsync(user);
                    if (!await _authorizationService.AuthorizeAsync(principal,
                        entity.CategoryId, Articles.Permissions.ViewSpamArticles))
                    {
                        result.Remove(user.Id);
                    }
                }

                // The entity is soft deleted but the user does 
                // not have permission to view soft deleted entities
                if (entity.IsDeleted)
                {
                    var principal = await _claimsPrincipalFactory.CreateAsync(user);
                    if (!await _authorizationService.AuthorizeAsync(principal,
                        entity.CategoryId, Articles.Permissions.ViewDeletedArticles))
                    {
                        result.Remove(user.Id);
                    }
                }

            }

            return result.Count > 0 ? result.Values : null;
            
        }

        bool FollowPostedValue()
        {            
            foreach (var key in _request.Form.Keys)
            {
                if (key == FollowHtmlName)
                {
                    var values = _request.Form[key];
                    if (!String.IsNullOrEmpty(values))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        bool NotifyPostedValue()
        {
            foreach (var key in _request.Form.Keys)
            {
                if (key == NotifyHtmlName)
                {
                    var values = _request.Form[key];
                    if (!String.IsNullOrEmpty(values))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

    }

}
