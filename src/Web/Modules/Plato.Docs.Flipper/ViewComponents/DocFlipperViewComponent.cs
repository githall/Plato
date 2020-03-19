using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plato.Docs.Flipper.ViewModels;
using Plato.Docs.Models;
using Plato.Entities.Services;
using Plato.Entities.ViewModels;
using PlatoCore.Data.Abstractions;
using PlatoCore.Security.Abstractions;

namespace Plato.Docs.Flipper.ViewComponents
{

    public class DocFlipperViewComponent : ViewComponent
    {
        
        private readonly ISimpleEntityService<SimpleDoc> _simpleEntityService;
        private readonly IAuthorizationService _authorizationService;    

        public DocFlipperViewComponent(
            ISimpleEntityService<SimpleDoc> simpleEntityService,
            IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
            _simpleEntityService = simpleEntityService;          
        }

        public async Task<IViewComponentResult> InvokeAsync(Doc entity, DocComment reply)
        {

            // Populate previous and next entity
            var model = await GetModelAsync(entity);

            // Return view
            return View(model);

        }


        async Task<DocFlipperViewModel> GetModelAsync(Doc entity)
        {

      
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            // Get all other entities at the same level as our current entity
            var entities = await _simpleEntityService
                .ConfigureQuery(async q =>
                {

                    q.FeatureId.Equals(entity.FeatureId);
                    q.ParentId.Equals(entity.ParentId);
                    q.CategoryId.Equals(entity.CategoryId);

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
                .GetResultsAsync(new EntityIndexOptions()
                {
                    Sort = SortBy.SortOrder,
                    Order = OrderBy.Asc
                });

            var model = new DocFlipperViewModel();

            // Return an empty model
            if (entities?.Data == null)
            {
                return model;
            }

            // Similar to entities.Data?
            //              .OrderByDescending(e => e.SortOrder)
            //              .FirstOrDefault(e => e.SortOrder < entity.SortOrder); ;
            // But avoiding LINQ for performance reasons
            for (var i = entities.Data.Count - 1; i >= 0; i--)
            {
                if (entities.Data[i].SortOrder < entity.SortOrder)
                {
                    model.PreviousDoc = entities.Data[i];
                    break;
                }
            }

            // Similar to FirstOrDefault(e => e.SortOrder > entity.SortOrder)
            // But avoiding LINQ for performance reasons
            foreach (var e in entities.Data)
            {
                if (e.SortOrder > entity.SortOrder)
                {
                    model.NextDoc = e;
                    break;
                }
            }

            return model;

           

        }


    }

}
