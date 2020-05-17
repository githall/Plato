﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Plato.Issues.Models;
using PlatoCore.Layout.ModelBinding;
using PlatoCore.Layout.ViewProviders.Abstractions;
using Plato.Entities.ViewModels;
using Plato.Entities.Models;
using Plato.Entities.Services;

namespace Plato.Issues.ViewProviders
{

    public class IssueViewProvider : ViewProviderBase<Issue>
    {

        private const string EditorHtmlName = "message";

        private readonly IEntityViewIncrementer<Issue> _viewIncrementer;
        private readonly HttpRequest _request;

        public IssueViewProvider(
            IEntityViewIncrementer<Issue> viewIncrementer,
            IHttpContextAccessor httpContextAccessor)
        {
            _request = httpContextAccessor.HttpContext.Request;
            _viewIncrementer = viewIncrementer;
        }

        public override Task<IViewProviderResult> BuildIndexAsync(Issue issue, IViewProviderContext context)
        {

            var viewModel = context.Controller.HttpContext.Items[typeof(EntityIndexViewModel<Issue>)] as EntityIndexViewModel<Issue>;
            if (viewModel == null)
            {
                throw new Exception($"A view model of type {typeof(EntityIndexViewModel<Entity>).ToString()} has not been registered on the HttpContext!");
            }

            return Task.FromResult(Views(
                View<EntityIndexViewModel<Issue>>("Home.Index.Header", model => viewModel).Zone("header"),
                View<EntityIndexViewModel<Issue>>("Home.Index.Tools", model => viewModel).Zone("header-right"),
                View<EntityIndexViewModel<Issue>>("Home.Index.Content", model => viewModel).Zone("content")
            ));

        }
        
        public override async Task<IViewProviderResult> BuildDisplayAsync(Issue issue, IViewProviderContext context)
        {
            
            var viewModel = context.Controller.HttpContext.Items[typeof(EntityViewModel<Issue, Comment>)] as EntityViewModel<Issue, Comment>;
            if (viewModel == null)
            {
                throw new Exception($"A view model of type {typeof(EntityViewModel<Issue, Comment>).ToString()} has not been registered on the HttpContext!");
            }

            // Increment entity views
            await _viewIncrementer
                .Contextulize(context.Controller.HttpContext)
                .IncrementAsync(issue);

            return Views(
                View<Issue>("Home.Display.Header", model => issue).Zone("header"),
                View<Issue>("Home.Display.Tools", model => issue).Zone("header-right"),                
                View<EntityViewModel<Issue, Comment>>("Home.Display.Content", model => viewModel).Zone("content"),
                View<Issue>("Home.Display.Sidebar", model => issue).Zone("content-right"),
                View<EditEntityReplyViewModel>("Home.Display.Footer", model => new EditEntityReplyViewModel()
                {
                    EntityId = issue.Id,
                    EditorHtmlName = EditorHtmlName
                }).Zone("resize-content"),
                View<EntityViewModel<Issue, Comment>>("Home.Display.Actions", model => viewModel)
                    .Zone("resize-actions-right")
                    .Order(int.MaxValue)
            );

        }

        public override Task<IViewProviderResult> BuildEditAsync(Issue issue, IViewProviderContext updater)
        {

            // Ensures we persist the message between post backs
            var message = issue.Message;
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
                Id = issue.Id,
                Title = issue.Title,
                Message = message,
                EditorHtmlName = EditorHtmlName,
                Alias = issue.Alias
            };

            return Task.FromResult(Views(
                View<EditEntityViewModel>("Home.Edit.Header", model => viewModel).Zone("header"),
                View<EditEntityViewModel>("Home.Edit.Content", model => viewModel).Zone("content"),
                View<EditEntityViewModel>("Home.Edit.Footer", model => viewModel).Zone("actions-right")
            ));

        }

        public override async Task<bool> ValidateModelAsync(Issue issue, IUpdateModel updater)
        {
            return await updater.TryUpdateModelAsync(new EditEntityViewModel
            {
                Title = issue.Title,
                Message = issue.Message
            });
        }

        public override async Task ComposeModelAsync(Issue issue, IUpdateModel updater)
        {

            var model = new EditEntityViewModel
            {
                Title = issue.Title,
                Message = issue.Message
            };

            await updater.TryUpdateModelAsync(model);

            if (updater.ModelState.IsValid)
            {

                issue.Title = model.Title;
                issue.Message = model.Message;
            }

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(Issue issue, IViewProviderContext context)
        {
            return await BuildEditAsync(issue, context);
        }

    }

}
