using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plato.Articles.Models;
using Plato.Entities.Services;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;
using PlatoCore.Layout.Views.Abstractions;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Security.Abstractions;

namespace Plato.Articles.ViewComponents
{

    public class ArticleCommentListViewComponent : ViewComponentBase
    {

        private readonly IEntityReplyStore<Comment> _entityReplyStore;
        private readonly IAuthorizationService _authorizationService;
        private readonly IEntityReplyService<Comment> _replyService;
        private readonly IEntityStore<Article> _entityStore;

        public ArticleCommentListViewComponent(
            IEntityReplyStore<Comment> entityReplyStore,
            IEntityStore<Article> entityStore,
            IEntityReplyService<Comment> replyService,
            IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
            _entityReplyStore = entityReplyStore;            
            _replyService = replyService;
            _entityStore = entityStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(EntityViewModel<Article, Comment> model)
        {

            if (model == null)
            {
                model = new EntityViewModel<Article, Comment>();
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

        async Task<EntityViewModel<Article, Comment>> GetViewModel(EntityViewModel<Article, Comment> model)
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
                        Permissions.ViewHiddenArticleComments))
                    {
                        q.HideHidden.True();
                    }

                    // Hide spam?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewSpamArticleComments))
                    {
                        q.HideSpam.True();
                    }

                    // Hide deleted?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewDeletedArticleComments))
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
