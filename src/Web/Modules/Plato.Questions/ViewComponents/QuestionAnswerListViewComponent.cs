using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plato.Questions.Models;
using Plato.Entities.Services;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Security.Abstractions;
using PlatoCore.Layout.Views.Abstractions;

namespace Plato.Questions.ViewComponents
{

    public class QuestionAnswerListViewComponent : ViewComponentBase
    {

        private readonly IAuthorizationService _authorizationService;
        private readonly IEntityReplyService<Answer> _replyService;
        private readonly IEntityStore<Question> _entityStore;

        public QuestionAnswerListViewComponent(
            IAuthorizationService authorizationService,
            IEntityReplyService<Answer> replyService,
            IEntityStore<Question> entityStore)
        {
            _authorizationService = authorizationService;            
            _replyService = replyService;
            _entityStore = entityStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(EntityViewModel<Question, Answer> model)
        {

            if (model == null)
            {
                model = new EntityViewModel<Question, Answer>();
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

        async Task<EntityViewModel<Question, Answer>> GetViewModel(EntityViewModel<Question, Answer> model)
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
                        Permissions.ViewHiddenAnswers))
                    {
                        q.HideHidden.True();
                    }

                    // Hide spam?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewSpamAnswers))
                    {
                        q.HideSpam.True();
                    }

                    // Hide deleted?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewDeletedAnswers))
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