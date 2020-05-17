﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Plato.Questions.Models;
using Plato.Questions.Services;
using Plato.Entities.Stores;
using PlatoCore.Layout.ModelBinding;
using PlatoCore.Layout.ViewProviders.Abstractions;
using Plato.Entities.ViewModels;
using Plato.Entities.Models;
using Plato.Entities.Services;

namespace Plato.Questions.ViewProviders
{
    public class QuestionViewProvider : ViewProviderBase<Question>
    {

        private const string EditorHtmlName = "message";
        
        private readonly IEntityStore<Question> _entityStore;
        private readonly IPostManager<Question> _articleManager;
        private readonly IEntityViewIncrementer<Question> _viewIncrementer;

        private readonly HttpRequest _request;
        
        public QuestionViewProvider(
            IHttpContextAccessor httpContextAccessor,
            IEntityStore<Question> entityStore,
            IPostManager<Question> articleManager,
            IEntityViewIncrementer<Question> viewIncrementer)
        {
            _entityStore = entityStore;
            _articleManager = articleManager;
            _viewIncrementer = viewIncrementer;
            _request = httpContextAccessor.HttpContext.Request;
        }

        public override Task<IViewProviderResult> BuildIndexAsync(Question question, IViewProviderContext context)
        {

            var viewModel = context.Controller.HttpContext.Items[typeof(EntityIndexViewModel<Question>)] as EntityIndexViewModel<Question>;
            if (viewModel == null)
            {
                throw new Exception($"A view model of type {typeof(EntityIndexViewModel<Entity>).ToString()} has not been registered on the HttpContext!");
            }

            return Task.FromResult(Views(
                View<EntityIndexViewModel<Question>>("Home.Index.Header", model => viewModel).Zone("header"),
                View<EntityIndexViewModel<Question>>("Home.Index.Tools", model => viewModel).Zone("header-right"),
                View<EntityIndexViewModel<Question>>("Home.Index.Content", model => viewModel).Zone("content")
            ));

        }
        
        public override async Task<IViewProviderResult> BuildDisplayAsync(Question question, IViewProviderContext context)
        {
            
            var viewModel = context.Controller.HttpContext.Items[typeof(EntityViewModel<Question, Answer>)] as EntityViewModel<Question, Answer>;
            if (viewModel == null)
            {
                throw new Exception($"A view model of type {typeof(EntityViewModel<Question, Answer>).ToString()} has not been registered on the HttpContext!");
            }

            // Increment entity views
            await _viewIncrementer
                .Contextulize(context.Controller.HttpContext)
                .IncrementAsync(question);

            return Views(
                View<Question>("Home.Display.Header", model => question).Zone("header"),
                View<Question>("Home.Display.Tools", model => question).Zone("header-right"),                
                View<EntityViewModel<Question, Answer>>("Home.Display.Content", model => viewModel).Zone("content"),
                View<Question>("Home.Display.Sidebar", model => question).Zone("content-right"),
                View<EditEntityReplyViewModel>("Home.Display.Footer", model => new EditEntityReplyViewModel()
                {
                    EntityId = question.Id,
                    EditorHtmlName = EditorHtmlName
                }).Zone("resize-content"),
                View<EntityViewModel<Question, Answer>>("Home.Display.Actions", model => viewModel)
                    .Zone("resize-actions-right")
                    .Order(int.MaxValue)

            );

        }
        
        public override Task<IViewProviderResult> BuildEditAsync(Question question, IViewProviderContext updater)
        {

            // Ensures we persist the message between post backs
            var message = question.Message;
            if (_request.Method == "POST")
            {
                foreach (string key in _request.Form.Keys)
                {
                    if (key == EditorHtmlName)
                    {
                        message = _request.Form[key];
                    }
                }
            }
          
            var viewModel = new EditEntityViewModel()
            {
                Id = question.Id,
                Title = question.Title,
                Message = message,
                EditorHtmlName = EditorHtmlName,
                Alias = question.Alias
            };

            return Task.FromResult(Views(
                View<EditEntityViewModel>("Home.Edit.Header", model => viewModel).Zone("header"),
                View<EditEntityViewModel>("Home.Edit.Content", model => viewModel).Zone("content"),
                View<EditEntityViewModel>("Home.Edit.Footer", model => viewModel).Zone("actions-right")
            ));

        }
        
        public override async Task<bool> ValidateModelAsync(Question question, IUpdateModel updater)
        {
            return await updater.TryUpdateModelAsync(new EditEntityViewModel
            {
                Title = question.Title,
                Message = question.Message
            });
        }

        public override async Task ComposeModelAsync(Question question, IUpdateModel updater)
        {

            var model = new EditEntityViewModel
            {
                Title = question.Title,
                Message = question.Message
            };

            await updater.TryUpdateModelAsync(model);

            if (updater.ModelState.IsValid)
            {

                question.Title = model.Title;
                question.Message = model.Message;
            }

        }
        
        public override async Task<IViewProviderResult> BuildUpdateAsync(Question question, IViewProviderContext context)
        {
            return await BuildEditAsync(question, context);
        }

    }

}
