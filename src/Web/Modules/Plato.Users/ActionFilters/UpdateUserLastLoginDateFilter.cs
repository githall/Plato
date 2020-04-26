using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PlatoCore.Hosting.Web.Abstractions;
using PlatoCore.Layout.ActionFilters;
using PlatoCore.Models.Reputations;
using PlatoCore.Models.Users;
using PlatoCore.Http.Abstractions;
using PlatoCore.Reputations.Abstractions;
using PlatoCore.Stores.Abstractions.Users;

namespace Plato.Users.ActionFilters
{

    public class UpdateUserLastLoginDateFilter : IModularActionFilter
    {

        private const int _sessionLength = 20;
        private const string _cookieName = "plato_active";
        private bool _active = false;

        private readonly IUserReputationAwarder _userReputationAwarder;
        private readonly IPlatoUserStore<User> _userStore;
        private readonly IContextFacade _contextFacade;
        private readonly ICookieBuilder _cookieBuilder;

        public UpdateUserLastLoginDateFilter(
            IUserReputationAwarder userReputationAwarder,
            IPlatoUserStore<User> userStore,
            ICookieBuilder cookieBuilder,
            IContextFacade contextFacade)
        {
            _userReputationAwarder = userReputationAwarder;
            _contextFacade = contextFacade;
            _cookieBuilder = cookieBuilder;
            _userStore = userStore;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {

            // Get tracking cookie
            var value = _cookieBuilder.Contextulize(context.HttpContext).Get(_cookieName);

            // Cookie does not exist
            if (String.IsNullOrEmpty(value))
            {
                _active = false;
                return;
            }

            // We have an active cookie, ensure this is known for the entire request
            _active = true;

        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            return;
        }

        public async Task OnActionExecutingAsync(ResultExecutingContext context)
        {

            // Not a view result
            if (!(context.Result is ViewResult))
            {
                return;
            }

            // Tracking cookie already exists, simply execute the controller result
            if (_active)
            {
                return;
            }

            // Get authenticated user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Not authenticated, simply execute the controller result
            if (user == null)
            {
                return;
            }

            user.Visits += 1;
            user.VisitsUpdatedDate = DateTimeOffset.UtcNow;
            user.LastLoginDate = DateTimeOffset.UtcNow;

            var result = await _userStore.UpdateAsync(user);
            if (result != null)
            {

                // Award visit reputation
                await _userReputationAwarder.AwardAsync(new Reputation("Visit", 1), result.Id, "Unique Visit");

                // Set client cookie to ensure update does not
                // occur again for as long as the cookie exists
                _cookieBuilder
                    .Contextulize(context.HttpContext)
                    .Append(
                        _cookieName,
                        true.ToString(),
                        new CookieOptions
                        {
                            HttpOnly = true,
                            Expires = DateTime.Now.AddMinutes(_sessionLength)
                        });

            }

        }

        public Task OnActionExecutedAsync(ResultExecutingContext context)
        {
            return Task.CompletedTask;
        }

    }

}
