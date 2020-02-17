using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Docs.Models;
using Microsoft.AspNetCore.Authorization;
using Plato.Entities.Extensions;
using Plato.Entities.Models;
using Plato.Entities.Services;
using Plato.Entities.ViewModels;
using PlatoCore.Data.Abstractions;
using PlatoCore.Features.Abstractions;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Security.Abstractions;
using PlatoCore.Layout.Views.Abstractions;

namespace Plato.Docs.ViewComponents
{

   public class DocTreeViewComponent : ViewComponentBase
    {

        private readonly IAuthorizationService _authorizationService;
        private readonly ISimpleEntityService<SimpleEntity> _simpleEntityService;
        private readonly IFeatureFacade _featureFacade;

        private bool? _displayMenu;

        public DocTreeViewComponent(
            ISimpleEntityService<SimpleEntity> simpleEntityService,
            IAuthorizationService authorizationService,            
            IFeatureFacade featureFacade)
        {
            _authorizationService = authorizationService;
            _simpleEntityService = simpleEntityService;
            _featureFacade = featureFacade;
        }

        public async Task<IViewComponentResult> InvokeAsync(EntityTreeOptions options)
        {

            if (options == null)
            {
                options = new EntityTreeOptions();;
            }

            // Get entities
            var entities = await GetSimpleEntitiesAsync(options.IndexOptions);

            // Add entities to view model
            options.Entities = entities?.Data?.BuildHierarchy<SimpleEntity>()?.OrderBy(r => r.SortOrder);

            // Do we have a menu to display?
            if (!string.IsNullOrEmpty(options.EditMenuViewName))
            {
                // Ensure we have permission to at least 1 of the menu options
                if (!_displayMenu.HasValue)
                {
                    await DisplayMenu();
                }
                 
                if (!_displayMenu.Value)
                {
                    // If we don't have permission disable menu
                    options.EditMenuViewName = string.Empty;
                }
            }

            // Return view
            return View(options);

        }

        private async Task<IPagedResults<SimpleEntity>> GetSimpleEntitiesAsync(EntityIndexOptions options)
        {

            // Set default feature
            if (options.FeatureId <= 0)
            {

                // Get feature
                var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Docs");

                // Ensure we found the feature
                if (feature == null)
                {
                    throw new Exception("The feature Plato.Docs could not be found");
                }
                
                options.FeatureId = feature.Id;

            }

            // Get results
            return await _simpleEntityService
                .ConfigureQuery(async q =>
                {

                    // Hide private?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewPrivateDocs))
                    {
                        q.HidePrivate.True();
                    }

                    // Hide hidden?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewHiddenDocs))
                    {
                        q.HideHidden.True();
                    }

                    // Hide spam?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewSpamDocs))
                    {
                        q.HideSpam.True();
                    }

                    // Hide deleted?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewDeletedDocs))
                    {
                        q.HideDeleted.True();
                    }
                    
                })
                .GetResultsAsync(options, new PagerOptions()
                {
                    Page = 1,
                    Size = int.MaxValue,
                    CountTotal = false
                });

        }

        private async Task DisplayMenu()
        {

            // Permissions needed for the menu
            var permissions = new List<IPermission>
            {
                Permissions.PostDocs,
                Permissions.EditAnyDoc,
                Permissions.SortAnyDoc,
                Permissions.DeleteAnyDoc
            };

            // If any of the permissions are allowed display menu
            foreach (var permission in permissions)
            {
                if (await _authorizationService.AuthorizeAsync(ViewContext.HttpContext.User, permission))
                {
                    _displayMenu = true;                    
                    return;
                }
            }

            _displayMenu = false;

        }

    }

}
