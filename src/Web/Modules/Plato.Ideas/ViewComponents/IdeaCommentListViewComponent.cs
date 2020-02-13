using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plato.Ideas.Models;
using Plato.Entities.Services;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Security.Abstractions;
using PlatoCore.Layout.Views.Abstractions;

namespace Plato.Ideas.ViewComponents
{

    public class IdeaCommentListViewComponent : ViewComponentBase
    {

        private readonly IEntityStore<Idea> _entityStore;
        private readonly IEntityReplyStore<IdeaComment> _entityReplyStore;
        private readonly IAuthorizationService _authorizationService;
        private readonly IEntityReplyService<IdeaComment> _replyService;

        public IdeaCommentListViewComponent(
            IEntityReplyStore<IdeaComment> entityReplyStore,
            IEntityStore<Idea> entityStore,
            IEntityReplyService<IdeaComment> replyService,
            IAuthorizationService authorizationService)
        {
            _entityReplyStore = entityReplyStore;
            _entityStore = entityStore;
            _replyService = replyService;
            _authorizationService = authorizationService;
        }

        public async Task<IViewComponentResult> InvokeAsync(EntityViewModel<Idea, IdeaComment> model)
        {

            if (model == null)
            {
                model = new EntityViewModel<Idea, IdeaComment>();
            }

            if (model.Options == null)
            {
                model.Options = new EntityOptions();
            }

            if (model.Pager == null)
            {
                model.Pager = new PagerOptions();
            }
            
            return View(await GetViewModel(model));

        }

        async Task<EntityViewModel<Idea, IdeaComment>> GetViewModel(EntityViewModel<Idea, IdeaComment> model)
        { 

            var entity = await _entityStore.GetByIdAsync(model.Options.Id);
            if (entity == null)
            {
                throw new ArgumentNullException();
            }

            var results = await _replyService
                .ConfigureQuery(async q =>
                {

                    // Hide private?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewHiddenIdeaComments))
                    {
                        q.HideHidden.True();
                    }

                    // Hide spam?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewSpamIdeaComments))
                    {
                        q.HideSpam.True();
                    }

                    // Hide deleted?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewDeletedIdeaComments))
                    {
                        q.HideDeleted.True();
                    }

                })
                .GetResultsAsync(model.Options, model.Pager);
            
            // Set total on pager
            model.Pager.SetTotal(results?.Total ?? 0);

            model.Entity = entity;
            model.Replies = results;

            return model;

        }

    }

}