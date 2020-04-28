using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;
using PlatoCore.Layout.ActionFilters;
using PlatoCore.Security.Abstractions;
using PlatoCore.Models.Users;

namespace Plato.Admin.ActionFilters
{

    /// <summary>
    /// An action filter that checks to ensure we have the
    /// necessary permissions to access any "Admin" controllers.
    /// </summary>
    public class AuthorizationFilter : IModularActionFilter
    {

        private readonly IAuthorizationService _authorizationService;

        public AuthorizationFilter(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            return;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            return;
        }

        public async Task OnActionExecutingAsync(ResultExecutingContext context)
        {

            // The controller action didn't return a view result so no need to continue execution
            if (!(context.Result is ViewResult result))
            {
                return;
            }

            // We need route values to check
            if (context.RouteData?.Values == null)
            {
                return;
            }

            // We need a controller name to check
            if (!context.RouteData.Values.ContainsKey("controller"))
            {
                return;
            }

            // If we are accessing an Admin controller check standard permissions
            var controllerName = context.RouteData.Values["controller"].ToString();
            switch (controllerName)
            {
                case "Admin":

                    // Unauthorized redirect
                    const string redirectRouteName = "UnauthorizedPage";
                    var redirectRouteValues = new RouteValueDictionary()
                    {
                        ["area"] = "Plato.Core",
                        ["controller"] = "Home",
                        ["action"] = "Denied"
                    };

                    // If we are not authenticated redirect to denied route immediately
                    if (!context.HttpContext.User.Identity.IsAuthenticated)
                    {
                        context.Result = new RedirectToRouteResult(redirectRouteName, redirectRouteValues);
                        return; // No need to continue execution
                    }

                    // Else check our claims 
                    if (!await _authorizationService.AuthorizeAsync(context.HttpContext.User, StandardPermissions.AdminAccess))
                    {
                        context.Result = new RedirectToRouteResult(redirectRouteName, redirectRouteValues);
                    }

                    break;

            }

        }

        public Task OnActionExecutedAsync(ResultExecutingContext context)
        {
            return Task.CompletedTask;
        }

    }

}
