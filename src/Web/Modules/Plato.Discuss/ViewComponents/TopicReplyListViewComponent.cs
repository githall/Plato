using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Models;
using Plato.Entities.Services;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;
using PlatoCore.Layout.Views.Abstractions;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Security.Abstractions;

namespace Plato.Discuss.ViewComponents
{

    public class TopicReplyListViewComponent : ViewComponentBase
    {

        private readonly IAuthorizationService _authorizationService;
        private readonly IEntityReplyService<Reply> _replyService;
        private readonly IEntityStore<Topic> _entityStore;

        public TopicReplyListViewComponent(
            IAuthorizationService authorizationService,
            IEntityReplyService<Reply> replyService,
            IEntityStore<Topic> entityStore)
        {
            _authorizationService = authorizationService;         
            _entityStore = entityStore;
            _replyService = replyService;         
        }

        public async Task<IViewComponentResult> InvokeAsync(EntityViewModel<Topic, Reply> model)
        {

            if (model == null)
            {
                model = new EntityViewModel<Topic, Reply>();
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

        async Task<EntityViewModel<Topic, Reply>> GetViewModel(EntityViewModel<Topic, Reply> model)
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
                        Permissions.ViewHiddenReplies))
                    {
                        q.HideHidden.True();
                    }

                    // Hide spam?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewSpamReplies))
                    {
                        q.HideSpam.True();
                    }

                    // Hide deleted?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewDeletedReplies))
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

            //// Return view model
            //return new EntityViewModel<Topic, Reply>
            //{
            //    Options = options,
            //    Pager = pager,
            //    Entity = entity,
            //    Replies = results
            //};

        }

    }

}
