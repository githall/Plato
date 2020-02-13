using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plato.Issues.Models;
using Plato.Entities.Services;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Security.Abstractions;
using PlatoCore.Layout.Views.Abstractions;

namespace Plato.Issues.ViewComponents
{

    public class IssueCommentListViewComponent : ViewComponentBase
    {

        private readonly IAuthorizationService _authorizationService;
        private readonly IEntityReplyService<Comment> _replyService;
        private readonly IEntityStore<Issue> _entityStore;

        public IssueCommentListViewComponent(            
            IEntityReplyService<Comment> replyService,
            IAuthorizationService authorizationService,
            IEntityStore<Issue> entityStore)
        {
            _authorizationService = authorizationService;
            _entityStore = entityStore;
            _replyService = replyService;     
        }

        public async Task<IViewComponentResult> InvokeAsync(EntityViewModel<Issue, Comment> model)
        {

            if (model == null)
            {
                model = new EntityViewModel<Issue, Comment>();
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

        async Task<EntityViewModel<Issue, Comment>> GetViewModel(EntityViewModel<Issue, Comment> model)
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
                        Permissions.ViewHiddenIssueComments))
                    {
                        q.HideHidden.True();
                    }

                    // Hide spam?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewSpamIssueComments))
                    {
                        q.HideSpam.True();
                    }

                    // Hide deleted?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewDeletedIssueComments))
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
        