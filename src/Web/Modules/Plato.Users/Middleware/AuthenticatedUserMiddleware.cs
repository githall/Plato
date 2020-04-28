﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Hosting.Web.Abstractions;
using PlatoCore.Models.Users;

namespace Plato.Users.Middleware
{

    // TODO: Move to action filter
    public class AuthenticatedUserMiddleware
    {
     
        private readonly RequestDelegate _next;

        public AuthenticatedUserMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task InvokeAsync(HttpContext context)
        {

            // Hydrate HttpContext.Features with our user
            await HydrateHttpContextFeatureAsync(context);

            // Return next delegate
            await _next.Invoke(context);
            
        }

        #region "Private Methods"

        async Task HydrateHttpContextFeatureAsync(HttpContext context)
        {

            // We are not authenticated
            if (!context.User.Identity.IsAuthenticated)
            {
                return;
            }

            // Get context facade
            var contextFacade = context.RequestServices.GetRequiredService<IContextFacade>();
            if (contextFacade == null)
            {
                return;
            }

            // Attempt tto get user from data store
            var user = await contextFacade.GetAuthenticatedUserAsync(context.User.Identity);

            // User not found
            if (user == null)
            {
                return;
            }
            
            // Add authenticated user to features for subsequent use
            context.Features[typeof(User)] = user;
            
        }
        
        #endregion

    }

}
