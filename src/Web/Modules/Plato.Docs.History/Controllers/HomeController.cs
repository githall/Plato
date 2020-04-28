﻿using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Entities.History.ViewModels;
using Plato.Docs.Models;
using Plato.Entities.History.Models;
using Plato.Entities.History.Services;
using Plato.Entities.History.Stores;
using Plato.Entities.Stores;
using PlatoCore.Data.Abstractions;
using PlatoCore.Hosting.Web.Abstractions;
using PlatoCore.Layout.Alerts;
using PlatoCore.Text.Abstractions.Diff;
using PlatoCore.Text.Abstractions.Diff.Models;
using PlatoCore.Security.Abstractions;
using Plato.Entities.Services;
using PlatoCore.Abstractions;
using PlatoCore.Models.Users;

namespace Plato.Docs.History.Controllers
{

    public class HomeController : Controller
    {

        private readonly IEntityHistoryManager<EntityHistory> _entityHistoryManager;
        private readonly IEntityHistoryStore<EntityHistory> _entityHistoryStore;
        private readonly IEntityReplyManager<DocComment> _entityReplyManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly IEntityReplyStore<DocComment> _entityReplyStore;
        private readonly IInlineDiffBuilder _inlineDiffBuilder;
        private readonly IEntityManager<Doc> _entityManager;
        private readonly IEntityStore<Doc> _entityStore;
        private readonly IContextFacade _contextFacade;
        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public HomeController(
            IStringLocalizer stringLocalizer,
            IHtmlLocalizer localizer,            
            IEntityReplyStore<DocComment> entityReplyStore,
            IEntityHistoryManager<EntityHistory> entityHistoryManager,
            IEntityHistoryStore<EntityHistory> entityHistoryStore,
            IEntityReplyManager<DocComment> entityReplyManager,            
            IAuthorizationService authorizationService,
            IInlineDiffBuilder inlineDiffBuilder,
            IEntityManager<Doc> entityManager,            
            IEntityStore<Doc> entityStore,
            IContextFacade contextFacade,
            IAlerter alerter)
        {
            _entityHistoryManager = entityHistoryManager;
            _authorizationService = authorizationService;
            _entityReplyManager = entityReplyManager;
            _entityHistoryStore = entityHistoryStore;
            _inlineDiffBuilder = inlineDiffBuilder;
            _entityReplyStore = entityReplyStore;
            _contextFacade = contextFacade;
            _entityManager = entityManager;
            _entityStore = entityStore;
            _alerter = alerter;

            T = localizer;
            S = stringLocalizer;

        }

        // --------------
        // Index
        // --------------

        public async Task<IActionResult> Index(int id)
        {

            // Get history point
            var history = await _entityHistoryStore.GetByIdAsync(id);
            if (history == null)
            {
                return NotFound();
            }

            // Get entity for history point
            var entity = await _entityStore.GetByIdAsync(history.EntityId);
            if (entity == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                   entity.CategoryId, history.EntityReplyId > 0
                    ? Permissions.viewReplyHistory
                    : Permissions.ViewEntityHistory))
            {
                return Unauthorized();
            }

            // Get previous history 
            var previousHistory = await _entityHistoryStore.QueryAsync()
                .Take(1, false)
                .Select<EntityHistoryQueryParams>(q =>
                {
                    q.Id.LessThan(history.Id);
                    q.EntityId.Equals(history.EntityId);
                    q.EntityReplyId.Equals(history.EntityReplyId);
                })
                .OrderBy("Id", OrderBy.Desc)
                .ToList();

            // Get newest / most recent history entry
            var latestHistory = await _entityHistoryStore.QueryAsync()
               .Take(1, false)
               .Select<EntityHistoryQueryParams>(q =>
               {
                   q.EntityId.Equals(history.EntityId);
                   q.EntityReplyId.Equals(history.EntityReplyId);
               })
               .OrderBy("Id", OrderBy.Desc)
               .ToList();

            // Compare previous to current
            var html = history.Html;
            if (previousHistory?.Data != null)
            {
                html = PrepareDifAsync(previousHistory.Data[0].Html, history.Html);
            }

            // Build model
            var viewModel = new HistoryIndexViewModel()
            {
                History = history,
                LatestHistory = latestHistory?.Data[0],
                Html = html
            };

            return View(viewModel);
        }

        // --------------
        // Rollback
        // --------------

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Rollback(int id)
        {

            // Validate
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            // Get history point
            var history = await _entityHistoryStore.GetByIdAsync(id);

            // Ensure we found the history point
            if (history == null)
            {
                return NotFound();
            }

            // Get entity for history point
            var entity = await _entityStore.GetByIdAsync(history.EntityId);

            // Ensure we found the entity
            if (entity == null)
            {
                return NotFound();
            }

            // Get reply
            DocComment reply = null;
            if (history.EntityReplyId > 0)
            {
                reply = await _entityReplyStore.GetByIdAsync(history.EntityReplyId);

                // Ensure we found a reply if supplied
                if (reply == null)
                {
                    return NotFound();
                }
            }

            // Get current user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // We always need to be logged in to edit entities
            if (user == null)
            {
                return Unauthorized();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                   entity.CategoryId, reply != null
                    ? Permissions.RevertReplyHistory
                    : Permissions.RevertEntityHistory))
            {
                return Unauthorized();
            }
            
            ICommandResultBase result;
            if (reply != null)
            {

                // Only update edited information if the message changes
                if (history.Message != reply.Message)
                {
                    reply.Message = history.Message;
                    reply.EditedUserId = user?.Id ?? 0;
                    reply.EditedDate = DateTimeOffset.UtcNow;
                }

                // Update reply to history point
                result = await _entityReplyManager.UpdateAsync(reply);

            }
            else
            {

                // Only update edited information if the message changes
                if (history.Message != entity.Message)
                {
                    entity.Message = history.Message;
                    entity.EditedUserId = user?.Id ?? 0;
                    entity.EditedDate = DateTimeOffset.UtcNow;
                }

                // Update entity to history point
                result = await _entityManager.UpdateAsync(entity);

            }

            // Add result
            if (result.Succeeded)
            {
                _alerter.Success(T["Version Rolled Back Successfully!"]);
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    _alerter.Danger(T[error.Description]);
                }
            }

            // Redirect
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Docs",
                ["controller"] = "Home",
                ["action"] = "Reply",
                ["opts.id"] = entity.Id,
                ["opts.alias"] = entity.Alias,
                ["opts.replyId"] = reply?.Id ?? 0
            }));

        }

        // --------------
        // Delete
        // --------------

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {

            // Validate
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            // Get history point
            var history = await _entityHistoryStore.GetByIdAsync(id);

            // Ensure we found the entity
            if (history == null)
            {
                return NotFound();
            }

            // Get entity
            var entity = await _entityStore.GetByIdAsync(history.EntityId);

            // Ensure we found the entity
            if (entity == null)
            {
                return NotFound();
            }

            // Get reply
            DocComment reply = null;
            if (history.EntityReplyId > 0)
            {
                reply = await _entityReplyStore.GetByIdAsync(history.EntityReplyId);
                // Ensure we found a reply if supplied
                if (reply == null)
                {
                    return NotFound();
                }
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                   entity.CategoryId, reply != null
                    ? Permissions.DeleteReplyHistory
                    : Permissions.DeleteEntityHistory))
            {
                return Unauthorized();
            }

            // Delete history point
            var result = await _entityHistoryManager.DeleteAsync(history);

            // Add result
            if (result.Succeeded)
            {

                // Update edit details for entity or reply based on latest history point
                var entityResult = await ApplyLatestHistoryPoint(entity, reply);
                if (entityResult.Succeeded)
                {
                    _alerter.Success(T["Version Deleted Successfully!"]);
                }
                else
                {
                    foreach (var error in entityResult.Errors)
                    {
                        _alerter.Danger(T[error.Description]);
                    }
                }

            }
            else
            {
                foreach (var error in result.Errors)
                {
                    _alerter.Danger(T[error.Description]);
                }
            }

            // Redirect
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Docs",
                ["controller"] = "Home",
                ["action"] = "Reply",
                ["opts.id"] = entity.Id,
                ["opts.alias"] = entity.Alias,
                ["opts.replyId"] = reply?.Id ?? 0
            }));

        }

        // --------------------

        async Task<ICommandResultBase> ApplyLatestHistoryPoint(Doc entity, DocComment reply)
        {

            // Get current user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // We need to be authenticated to make changes
            if (user == null)
            {
                return await ResetEditDetails(entity, reply, user);
            }

            // Get newest / most recent history entry
            var histories = await _entityHistoryStore.QueryAsync()
               .Take(1, false)
               .Select<EntityHistoryQueryParams>(q =>
               {
                   q.EntityId.Equals(entity.Id);
                   q.EntityReplyId.Equals(reply?.Id ?? 0);
               })
               .OrderBy("Id", OrderBy.Desc)
               .ToList();

            // No history point, return success
            if (histories == null)
            {
                return await ResetEditDetails(entity, reply, user);
            }

            // No history point, return success
            if (histories.Data == null)
            {
                return await ResetEditDetails(entity, reply, user);
            }

            // No history point, return success
            if (histories.Data.Count == 0)
            {
                return await ResetEditDetails(entity, reply, user);
            }

            var history = histories.Data[0];

            // No history available reset edit details
            if (history == null)
            {
                return await ResetEditDetails(entity, reply, user);
            }

            // Update edit details based on latest history point

            var result = new CommandResultBase();
            
            if (reply != null)
            {

                reply.ModifiedUserId = user.Id;
                reply.ModifiedDate = DateTimeOffset.UtcNow;
                reply.EditedUserId = history.CreatedUserId;
                reply.EditedDate = history.CreatedDate;

                // Update reply to history point
                var updateResult = await _entityReplyStore.UpdateAsync(reply);
                if (updateResult != null)
                {
                    return result.Success();
                }

            }
            else
            {

                entity.ModifiedUserId = user.Id;
                entity.ModifiedDate = DateTimeOffset.UtcNow;
                entity.EditedUserId = history.CreatedUserId;
                entity.EditedDate = history.CreatedDate;

                // Update entity to history point
                var updateResult = await _entityStore.UpdateAsync(entity);
                if (updateResult != null)
                {
                    return result.Success();
                }

            }

            return result.Success();

        }

        async Task<ICommandResultBase> ResetEditDetails(Doc entity, DocComment reply, IUser user)
        {

            var result = new CommandResultBase();
            
            // We need to be authenticated to make changes
            if (user == null)
            {
                return result.Success();
            }
                        
            if (reply != null)
            {

                reply.ModifiedUserId = user.Id;
                reply.ModifiedDate = DateTimeOffset.UtcNow;
                reply.EditedUserId = 0;
                reply.EditedDate = null;

                var updateResult = await _entityReplyStore.UpdateAsync(reply);
                if (updateResult != null)
                {
                    return result.Success();
                }
            }
            else
            {

                entity.ModifiedUserId = user.Id;
                entity.ModifiedDate = DateTimeOffset.UtcNow;
                entity.EditedUserId = 0;
                entity.EditedDate = null;

                var updateResult = await _entityStore.UpdateAsync(entity);
                if (updateResult != null)
                {
                    return result.Success();
                }
            }

            return result.Success();

        }

        string PrepareDifAsync(string before, string after)
        {

            var sb = new StringBuilder();
            var diff = _inlineDiffBuilder.BuildDiffModel(before, after);

            foreach (var line in diff.Lines)
            {
                switch (line.Type)
                {
                    case ChangeType.Inserted:
                        sb.Append("<div class=\"inserted-line\">");
                        sb.Append(line.Text);
                        sb.Append("</div>");
                        break;
                    case ChangeType.Deleted:
                        sb.Append("<div class=\"deleted-line\">");
                        sb.Append(line.Text);
                        sb.Append("</div>");
                        break;
                    case ChangeType.Imaginary:
                        sb.Append("<div class=\"imaginary-line\">");
                        sb.Append(line.Text);
                        sb.Append("</div>");
                        break;
                    case ChangeType.Modified:
                        foreach (var character in line.SubPieces)
                        {
                            if (character.Type == ChangeType.Imaginary)
                            {
                                continue;
                            }
                            sb.Append("<span class=\"")
                                .Append(character.Type.ToString().ToLower())
                                .Append("-character\">")
                                .Append(character.Text)
                                .Append("</span>");
                        }
                        break;
                    default:
                        sb.Append(line.Text);
                        break;
                }

            }

            return sb.ToString();

        }

    }

}
