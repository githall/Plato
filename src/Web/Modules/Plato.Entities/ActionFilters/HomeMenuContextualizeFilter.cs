﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Plato.Entities.Models;
using Plato.Entities.Services;
using PlatoCore.Layout;
using PlatoCore.Layout.ActionFilters;
using PlatoCore.Security.Abstractions;

namespace Plato.Entities.ActionFilters
{

    public class HomeMenuContextualizeFilter : IModularActionFilter
    {
        
        private readonly IFeatureEntityCountService _featureEntityCountService;        
        private readonly IAuthorizationService _authorizationService;

        public HomeMenuContextualizeFilter(            
            IFeatureEntityCountService featureEntityCountService,
            IAuthorizationService authorizationService)
        {            
            _featureEntityCountService = featureEntityCountService;
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
            
            // Check early to ensure we are working with a LayoutViewModel
            if (!(result.Model is LayoutViewModel model))
            {
                return;
            }
            
            // We need route values to check
            if (context.RouteData?.Values == null)
            {
                return;
            }
            
            // We need an area
            if (!context.RouteData.Values.ContainsKey("area"))
            {
                return;
            }

            // We need a controller
            if (!context.RouteData.Values.ContainsKey("controller"))
            {
                return;
            }

            // We need an action
            if (!context.RouteData.Values.ContainsKey("action"))
            {
                return;
            }

            // Plato.Core?
            var isArea = context.RouteData.Values["area"].ToString()
                .Equals("Plato.Core", StringComparison.OrdinalIgnoreCase);

            // Not area
            if (!isArea)
            {
                return;
            }
            
            // Home controller?
            var isController = context.RouteData.Values["controller"].ToString()
                .Equals("Home", StringComparison.OrdinalIgnoreCase);

            // Not controller
            if (!isController)
            {
                return;
            }

            // Index action?
            var isAction = context.RouteData.Values["action"].ToString()
                .Equals("Index", StringComparison.OrdinalIgnoreCase);
            
            // Not action
            if (!isAction)
            {
                return;
            }
            
            // We are on the homepage, register metrics on context
            context.HttpContext.Items[typeof(FeatureEntityCounts)] = new FeatureEntityCounts()
            {
                Features = await _featureEntityCountService
                    .ConfigureQuery(async q =>
                    {

                        // Hide private?
                        if (!await _authorizationService.AuthorizeAsync(context.HttpContext.User,
                            Permissions.ViewPrivateEntities))
                        {
                            q.HidePrivate.True();
                        }

                        // Hide hidden?
                        if (!await _authorizationService.AuthorizeAsync(context.HttpContext.User,
                            Permissions.ViewHiddenEntities))
                        {
                            q.HideHidden.True();
                        }

                        // Hide spam?
                        if (!await _authorizationService.AuthorizeAsync(context.HttpContext.User,
                            Permissions.ViewSpamEntities))
                        {
                            q.HideSpam.True();
                        }

                        // Hide deleted?
                        if (!await _authorizationService.AuthorizeAsync(context.HttpContext.User,
                            Permissions.ViewDeletedEntities))
                        {
                            q.HideDeleted.True();
                        }

                    })
                    .GetResultsAsync(null)
            };

        }

        public Task OnActionExecutedAsync(ResultExecutingContext context)
        {
            return Task.CompletedTask;
        }
    }

}
