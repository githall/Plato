using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plato.Docs.Models;
using Plato.Entities.Models;
using Plato.Entities.Services;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;
using PlatoCore.Data.Abstractions;
using PlatoCore.Layout.Views.Abstractions;
using PlatoCore.Security.Abstractions;

namespace Plato.Docs.ViewComponents
{

    public class DocViewComponent : ViewComponentBase
    {

        private readonly IAuthorizationService _authorizationService;
        private readonly ISimpleEntityService<SimpleEntity> _simpleEntityService;
        private readonly IEntityStore<Doc> _entityStore;

        public DocViewComponent(            
            ISimpleEntityService<SimpleEntity> simpleEntityService,
            IAuthorizationService authorizationService,
            IEntityStore<Doc> entityStore)
        {
            _authorizationService = authorizationService;
            _simpleEntityService = simpleEntityService;
            _entityStore = entityStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(EntityViewModel<Doc, DocComment> model)
        {

            if (model == null)
            {
                model = new EntityViewModel<Doc, DocComment>();
            }

            if (model.Options == null)
            {
                model.Options = new EntityOptions();
            }

            return View(await GetViewModel(model));

        }

        async Task<EntityViewModel<Doc, DocComment>> GetViewModel(EntityViewModel<Doc, DocComment> model)
        {

            if (model.Entity == null)
            {

                if (model.Options.Id <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(model.Options.Id));
                }

                // Get entity
                var entity = await _entityStore.GetByIdAsync(model.Options.Id);

                if (entity == null)
                {
                    throw new ArgumentNullException();
                }

                model.Entity = entity;

            }

            // Populate previous and next entity
            await PopulatePreviousAndNextAsync(model.Entity);

            // Populate child entities
            await PopulateChildEntitiesAsync(model.Entity);

            return model;

        }

        async Task PopulatePreviousAndNextAsync(Doc entity)
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

            // Get the previous and next entities via the sort order
            if (entities != null)
            {

                if (entities.Data == null)
                {
                    return;
                }

                // Similar to entities.Data?
                //              .OrderByDescending(e => e.SortOrder)
                //              .FirstOrDefault(e => e.SortOrder < entity.SortOrder); ;
                // But avoiding LINQ for performance reasons
                for (var i = entities.Data.Count - 1; i >= 0; i--)
                {                  
                    if (entities.Data[i].SortOrder < entity.SortOrder)
                    {
                        entity.PreviousDoc = entities.Data[i];
                        break;
                    }
                }

                // Similar to FirstOrDefault(e => e.SortOrder > entity.SortOrder)
                // But avoiding LINQ for performance reasons
                foreach (var e in entities.Data)
                {
                    if (e.SortOrder > entity.SortOrder)
                    {
                        entity.NextDoc = e;
                        break;
                    }
                }

            }

        }

        async Task PopulateChildEntitiesAsync(Doc entity)
        {

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            // Get all child entities
            var entities = await _simpleEntityService
                .ConfigureQuery(async q =>
                {
                    q.ParentId.Equals(entity.Id);

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

            // Get the previous and next entities via the sort order
            entity.ChildEntities = entities?.Data;

        }

    }

}
