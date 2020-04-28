﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Data.Abstractions;
using PlatoCore.Notifications.Abstractions;
using Plato.Notifications.Models;
using Plato.Notifications.Stores;
using Plato.WebApi.Attributes;
using Plato.WebApi.Controllers;
using Plato.WebApi.Models;
using PlatoCore.Hosting.Web.Abstractions;

namespace Plato.Notifications.Controllers
{

    public class UserController : BaseWebApiController
    {

        private readonly IUserNotificationsStore<UserNotification> _userNotificationStore;
        private readonly IContextFacade _contextFacade;

        public UserController(
            IUserNotificationsStore<UserNotification> userNotificationStore,
            IContextFacade contextFacade)
        {
            _userNotificationStore = userNotificationStore;
            _contextFacade = contextFacade;            
        }

        #region "Actions"

        /// <summary>
        /// Get all notifications for the authenticated user.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="sort"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpGet, ResponseCache(NoStore = true)]
        public async Task<IActionResult> Get(
            int page = 1,
            int size = 10,
            string sort = "CreatedDate",
            OrderBy order = OrderBy.Desc)
        {

            // Ensure we are authenticated
            var user = await base.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return base.UnauthorizedException();
            }

            // Get notifications
            var userNotifications = await GetUserNotifications(
                page,
                size,
                user.Id,
                sort,
                order);

            IPagedResults<UserNotificationApiResult> results = null;
            if (userNotifications != null)
            {
                results = new PagedResults<UserNotificationApiResult>
                {
                    Total = userNotifications.Total
                };

                foreach (var userNotification in userNotifications.Data)
                {

                    var toUrl = _contextFacade.GetRouteUrl(new RouteValueDictionary()
                    {
                        ["area"] = "Plato.Users",
                        ["controller"] = "Home",
                        ["action"] = "Display",
                        ["opts.id"] = userNotification.To.Id,
                        ["opts.alias"] = userNotification.To.Alias
                    });

                    var fromUrl = _contextFacade.GetRouteUrl(new RouteValueDictionary()
                    {
                        ["area"] = "Plato.Users",
                        ["controller"] = "Home",
                        ["action"] = "Display",
                        ["opts.id"] = userNotification.From.Id,
                        ["opts.alias"] = userNotification.From.Alias
                    });

                    if (string.IsNullOrEmpty(userNotification.To.Avatar.Url))
                    {
                        userNotification.To.Avatar.Url = _contextFacade.GetRouteUrl(userNotification.To.Avatar.DefaultRoute);
                    }

                    if (string.IsNullOrEmpty(userNotification.From.Avatar.Url))
                    {
                        userNotification.From.Avatar.Url = _contextFacade.GetRouteUrl(userNotification.From.Avatar.DefaultRoute);
                    }

                    results.Data.Add(new UserNotificationApiResult()
                    {
                        Id = userNotification.Id,
                        To = new UserApiResult()
                        {
                            Id = userNotification.To.Id,
                            DisplayName = userNotification.To.DisplayName,
                            UserName = userNotification.To.UserName,
                            Avatar = userNotification.To.Avatar,
                            Url = toUrl
                        },
                        From = new UserApiResult()
                        {
                            Id = userNotification.From.Id,
                            DisplayName = userNotification.From.DisplayName,
                            UserName = userNotification.From.UserName,
                            Avatar = userNotification.From.Avatar,
                            Url = fromUrl
                        },
                        Title = userNotification.Title,
                        Message = userNotification.Message,
                        Url = userNotification.Url,
                        Date = new FriendlyDate()
                        {
                            Text = userNotification.CreatedDate.ToPrettyDate(),
                            Value = userNotification.CreatedDate
                        }
                    });

                }

            }

            IPagedApiResults<UserNotificationApiResult> output = null;
            if (results != null)
            {
                output = new PagedApiResults<UserNotificationApiResult>()
                {
                    Page = page,
                    Size = size,
                    Total = results.Total,
                    TotalPages = results.Total.ToSafeCeilingDivision(size),
                    Data = results.Data
                };
            }

            return output != null
                ? base.Result(output)
                : base.NoResults();

        }

        /// <summary>
        /// Get the total number of notifications that have no read date. 
        /// </summary>
        /// <returns></returns>
        [HttpGet, ResponseCache(NoStore = true)]
        public async Task<IActionResult> Unread()
        {

            // Ensure we are authenticated
            var user = await base.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return base.UnauthorizedException();
            }

            // Get total unread message count
            var unread = await _userNotificationStore.QueryAsync()
                .Take(1)
                .Select<UserNotificationsQueryParams>(q =>
                {
                    q.UserId.Equals(user.Id);
                    q.HideRead.True();
                })
                .ToList();

            return unread != null
                ? base.Result(unread.Total.ToPrettyInt())
                : base.Result(0);

        }

        /// <summary>
        /// Mark all notifications as read.
        /// </summary>
        /// <returns></returns>
        [HttpPost, ValidateClientAntiForgeryToken, ResponseCache(NoStore = true)]
        public async Task<IActionResult> MarkRead()
        {

            // Ensure we are authenticated
            var user = await base.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return base.UnauthorizedException();
            }
            
            return await _userNotificationStore.UpdateReadDateAsync(user.Id, DateTimeOffset.UtcNow)
                ? base.Result(true)
                : base.Result(false);

        }

        /// <summary>
        /// Delete a notification that belongs to the currently authenticated user.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete, ResponseCache(NoStore = true)]
        public async Task<IActionResult> Delete(int id)
        {

            // Ensure notification exists
            var userNotification = await _userNotificationStore.GetByIdAsync(id);
            if (userNotification == null)
            {
                return base.NotFound();
            }

            // Ensure we are authenticated
            var user = await base.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return base.UnauthorizedException();
            }

            // Ensure we are deleting a notification we own
            if (userNotification.UserId != user.Id)
            {
                return base.UnauthorizedException();
            }

            var success = await _userNotificationStore.DeleteAsync(userNotification);
            if (success)
            {
                return base.Result("Notification deleted successfully");
            }

            return base.InternalServerError();

        }
        
        #endregion

        #region "Private Methods"

        async Task<IPagedResults<UserNotification>> GetUserNotifications(
            int page,
            int pageSize,
            int userId,
            string sortBy,
            OrderBy sortOrder)
        {
            return await _userNotificationStore.QueryAsync()
                .Take(page, pageSize, true)
                .Select<UserNotificationsQueryParams>(q => { q.UserId.Equals(userId); })
                .OrderBy(sortBy, sortOrder)
                .ToList();
        }

        #endregion

    }

}
