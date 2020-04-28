﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Articles.Models;
using Plato.Articles.Services;
using Plato.Entities;
using Plato.Entities.Models;
using Plato.Entities.Services;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;
using PlatoCore.Abstractions;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Data.Abstractions;
using PlatoCore.Features.Abstractions;
using PlatoCore.Hosting.Web.Abstractions;
using PlatoCore.Layout;
using PlatoCore.Layout.Alerts;
using PlatoCore.Layout.ModelBinding;
using PlatoCore.Layout.Titles;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Http.Abstractions;
using PlatoCore.Security.Abstractions;

namespace Plato.Articles.Controllers
{
    public class HomeController : Controller, IUpdateModel
    {

        #region "Constructor"

        private readonly IReportEntityManager<Article> _reportEntityManager;
        private readonly IReportEntityManager<Comment> _reportReplyManager;
        private readonly IViewProviderManager<Article> _entityViewProvider;
        private readonly IViewProviderManager<Comment> _replyViewProvider;
        private readonly IEntityReplyStore<Comment> _entityReplyStore;        
        private readonly IAuthorizationService _authorizationService;
        private readonly IEntityReplyService<Comment> _replyService;        
        private readonly IPostManager<Article> _articleManager;
        private readonly IPostManager<Comment> _commentManager;
        private readonly IEntityService<Article> _articleService;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IPageTitleBuilder _pageTitleBuilder;
        private readonly IEntityStore<Article> _entityStore;
        private readonly IClientIpAddress _clientIpAddress;
        private readonly IFeatureFacade _featureFacade;
        private readonly IContextFacade _contextFacade;
        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public HomeController(
            IStringLocalizer<HomeController> stringLocalizer,
            IHtmlLocalizer<HomeController> localizer,
            IReportEntityManager<Article> reportEntityManager,
            IReportEntityManager<Comment> reportReplyManager,
            IViewProviderManager<Article> entityViewProvider,
            IViewProviderManager<Comment> replyViewProvider,
            IEntityReplyStore<Comment> entityReplyStore,
            IAuthorizationService authorizationService,
            IEntityReplyService<Comment> replyService,
            IEntityService<Article> articleService,
            IPostManager<Article> articleManager,            
            IPostManager<Comment> commentManager,            
            IBreadCrumbManager breadCrumbManager,
            IPageTitleBuilder pageTitleBuilder,
            IEntityStore<Article> entityStore,
            IClientIpAddress clientIpAddress,
            IFeatureFacade featureFacade,
            IContextFacade contextFacade,
            IAlerter alerter)
        {

            _authorizationService = authorizationService;
            _reportEntityManager = reportEntityManager;            
            _reportReplyManager = reportReplyManager;
            _entityViewProvider = entityViewProvider;
            _breadCrumbManager = breadCrumbManager;
            _replyViewProvider = replyViewProvider;
            _pageTitleBuilder = pageTitleBuilder;            
            _entityReplyStore = entityReplyStore;                                   
            _clientIpAddress = clientIpAddress;
            _articleManager = articleManager;
            _commentManager = commentManager;
            _articleService = articleService;
            _contextFacade = contextFacade;
            _featureFacade = featureFacade;
            _replyService = replyService;            
            _entityStore = entityStore;
            _alerter = alerter;

            T = localizer;
            S = stringLocalizer;

        }

        #endregion

        #region "Actions"

        // -----------------
        // Index
        // -----------------

        public async Task<IActionResult> Index(EntityIndexOptions opts, PagerOptions pager)
        {

            // default options
            if (opts == null)
            {
                opts = new EntityIndexOptions();
            }

            // default pager
            if (pager == null)
            {
                pager = new PagerOptions();
            }

            // Get default options
            var defaultViewOptions = new EntityIndexOptions();
            var defaultPagerOptions = new PagerOptions();

            // Add non default route data for pagination purposes
            if (opts.Search != defaultViewOptions.Search)
                this.RouteData.Values.Add("opts.search", opts.Search);
            if (opts.Sort != defaultViewOptions.Sort)
                this.RouteData.Values.Add("opts.sort", opts.Sort);
            if (opts.Order != defaultViewOptions.Order)
                this.RouteData.Values.Add("opts.order", opts.Order);
            if (opts.Filter != defaultViewOptions.Filter)
                this.RouteData.Values.Add("opts.filter", opts.Filter);
            if (pager.Page != defaultPagerOptions.Page)
                this.RouteData.Values.Add("pager.page", pager.Page);
            if (pager.Size != defaultPagerOptions.Size)
                this.RouteData.Values.Add("pager.size", pager.Size);
            
            // Build view model
            var viewModel = await GetIndexViewModelAsync(opts, pager);

            // Add view model to context
            HttpContext.Items[typeof(EntityIndexViewModel<Article>)] = viewModel;
            
            // If we have a pager.page querystring value return paged results
            if (int.TryParse(HttpContext.Request.Query["pager.page"], out var page))
            {
                if (page > 0)
                    return View("GetArticles", viewModel);
            }

            // Return Url for authentication purposes
            ViewData["ReturnUrl"] = _contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Articles",
                ["controller"] = "Home",
                ["action"] = "Index"
            });

            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Articles"]);
            });
            
            // Return view
            return View((LayoutViewModel) await _entityViewProvider.ProvideIndexAsync(new Article(), this));

        }

        // -----------------
        // New Entity
        // -----------------

        public async Task<IActionResult> Create(int channel)
        {

            if (!await _authorizationService.AuthorizeAsync(this.User, channel, Permissions.PostArticles))
            {
                return Unauthorized();
            }

            var entity = new Article();
            if (channel > 0)
            {
                entity.CategoryId = channel;
            }

            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Articles"], articles => articles
                    .Action("Index", "Home", "Plato.Articles")
                    .LocalNav()
                ).Add(S["New Article"], post => post
                    .LocalNav()
                );
            });

            // Return view
            return View((LayoutViewModel) await _entityViewProvider.ProvideEditAsync(entity, this));

        }

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Create))]
        public async Task<IActionResult> CreatePost(EditEntityViewModel model)
        {

            // Get authenticated user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Build entity
            var entity = new Article()
            {
                Title = model.Title,
                Message = model.Message,
                CreatedUserId = user?.Id ?? 0,
                CreatedDate = DateTimeOffset.UtcNow,
                IpV4Address = _clientIpAddress.GetIpV4Address(),
                IpV6Address = _clientIpAddress.GetIpV6Address()
            };

            // Validate model state within all view providers
            if (await _entityViewProvider.IsModelStateValidAsync(entity, this))
            {

                // Get composed model from all involved view providers
                entity = await _entityViewProvider.ComposeModelAsync(entity, this);

                // Ensure we have permission 
                if (!await _authorizationService.AuthorizeAsync(this.User, entity.CategoryId, Permissions.PostArticles))
                {
                    return Unauthorized();
                }

                // Create entity
                var result = await _articleManager.CreateAsync(entity);

                // Ensure the insert was successful
                if (result.Succeeded)
                {

                    // Indicate new entity
                    result.Response.IsNew = true;

                    // Execute view providers ProvideUpdateAsync method
                    await _entityViewProvider.ProvideUpdateAsync(result.Response, this);
                    
                    // Get authorize result
                    var authorizeResult = await AuthorizeAsync(result.Response);
                    if (authorizeResult.Succeeded)
                    {
                        // Everything was OK
                        _alerter.Success(T["Article Added Successfully!"]);

                        // Redirect to entity
                        return RedirectToAction(nameof(Display), new RouteValueDictionary()
                        {
                            ["opts.id"] = result.Response.Id,
                            ["opts.alias"] = result.Response.Alias
                        });

                    }
                    
                    // Add any authorization errors
                    foreach (var error in authorizeResult.Errors)
                    {
                        _alerter.Success(T[error.Description]);
                    }

                    // Redirect to index
                    return RedirectToAction(nameof(Index));

                }
                else
                {
                    // Errors that may have occurred whilst creating the entity
                    foreach (var error in result.Errors)
                    {
                        ViewData.ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

            }
            
            return await Create(0);

        }

        // -----------------
        // Display Entity
        // -----------------

        public async Task<IActionResult> Display(EntityOptions opts, PagerOptions pager)
        {


            // Default options
            if (opts == null)
            {
                opts = new EntityOptions();
            }

            // Default pager
            if (pager == null)
            {
                pager = new PagerOptions();
            }
            
            // We always need an entity Id to display
            if (opts.Id <= 0)
            {
                return NotFound();
            }

            // Get the entity
            var entity = await GetEntityAsync(opts.Id);

            // We don't have permission or the entity does not exist
            if (entity == null)
            {
                // Return a 404 if the entity does not exist
                // Return a 401 to indicate a authorization issue
                return await _entityStore.GetByIdAsync(opts.Id) == null
                    ? (IActionResult)NotFound()
                    : (IActionResult)Unauthorized();
            }

            // Maintain previous route data when generating page links
            var defaultViewOptions = new EntityViewModel<Article, Comment>();
            var defaultPagerOptions = new PagerOptions();
            
            if (pager.Page != defaultPagerOptions.Page && !this.RouteData.Values.ContainsKey("pager.page"))
                this.RouteData.Values.Add("pager.page", pager.Page);
            if (pager.Size != defaultPagerOptions.Size && !this.RouteData.Values.ContainsKey("pager.size"))
                this.RouteData.Values.Add("pager.size", pager.Size);
            
            // Build view model
            var viewModel = GetDisplayViewModel(entity, opts, pager);

            // Add models to context
            HttpContext.Items[typeof(EntityViewModel<Article, Comment>)] = viewModel;
            HttpContext.Items[typeof(Article)] = entity;
            
            // If we have a pager.page querystring value return paged results
            if (int.TryParse(HttpContext.Request.Query["pager.page"], out var page))
            {
                if (page > 0)
                {
                    return View("GetArticleComments", viewModel);
                }
            }

            // Return Url for authentication purposes
            ViewData["ReturnUrl"] = _contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Articles",
                ["controller"] = "Home",
                ["action"] = "Display",
                ["opts.id"] = entity.Id,
                ["opts.alias"] = entity.Alias
            });

            // Build page title
            _pageTitleBuilder.AddSegment(S[entity.Title], int.MaxValue);

            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Articles"], articles => articles
                    .Action("Index", "Home", "Plato.Articles")
                    .LocalNav()
                ).Add(S[entity.Title.TrimToAround(75)], post => post
                    .LocalNav()
                );
            });

            // Return view
            return View((LayoutViewModel) await _entityViewProvider.ProvideDisplayAsync(entity, this));

        }

        // -----------------
        // Post Reply
        // -----------------

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Display))]
        public async Task<IActionResult> DisplayPost(EditEntityReplyViewModel model)
        {

            // Get entity
            var entity = await _entityStore.GetByIdAsync(model.EntityId);

            // Ensure entity exists
            if (entity == null)
            {
                return NotFound();
            }

            // Ensure we have permission 
            if (!await _authorizationService.AuthorizeAsync(this.User, entity.CategoryId, Permissions.PostArticleComments))
            {
                return Unauthorized();
            }

            // Get authenticated user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Build reply
            var reply = new Comment()
            {
                EntityId = model.EntityId,
                Message = model.Message,
                CreatedUserId = user?.Id ?? 0,
                CreatedDate = DateTimeOffset.UtcNow,
                IpV4Address = _clientIpAddress.GetIpV4Address(),
                IpV6Address = _clientIpAddress.GetIpV6Address()
            };

            // Validate model state within all view providers
            if (await _replyViewProvider.IsModelStateValidAsync(reply, this))
            {

                // Get composed type from all involved view providers
                reply = await _replyViewProvider.ComposeModelAsync(reply, this);

                // We need to first add the reply so we have a unique Id
                // for all ProvideUpdateAsync methods within any involved view providers
                var result = await _commentManager.CreateAsync(reply);

                // Ensure the insert was successful
                if (result.Succeeded)
                {

                    // Indicate this is a new reply so our view provider won't attempt to update
                    result.Response.IsNewReply = true;

                    // Execute view providers ProvideUpdateAsync method
                    await _replyViewProvider.ProvideUpdateAsync(result.Response, this);
                    
                    // Get authorization result
                    var authorizeResult = await AuthorizeAsync(result.Response);
                    if (authorizeResult.Succeeded)
                    {

                        // Everything was OK
                        _alerter.Success(T["Comment Added Successfully!"]);

                        // Redirect to reply
                        return RedirectToAction(nameof(Reply), new RouteValueDictionary()
                        {
                            ["opts.id"] = entity.Id,
                            ["opts.alias"] = entity.Alias,
                            ["opts.replyId"] = result.Response.Id
                        });
                    }

                    // Add authorization errors
                    foreach (var error in authorizeResult.Errors)
                    {
                        _alerter.Success(T[error.Description]);
                    }

                    // Redirect to entity
                    return RedirectToAction(nameof(Display), new RouteValueDictionary()
                    {
                        ["opts.id"] = entity.Id,
                        ["opts.alias"] = entity.Alias
                    });

                }
                else
                {
                    // Errors that may have occurred whilst creating the entity
                    foreach (var error in result.Errors)
                    {
                        ViewData.ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

            }

            // if we reach this point some view model validation
            // failed within a view provider, display model state errors
            foreach (var modelState in ViewData.ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    _alerter.Danger(T[error.ErrorMessage]);
                }
            }

            return await Display(new EntityOptions()
            {
                Id = entity.Id
            }, null);

        }
      
        // -----------------
        // Edit Entity
        // -----------------

        public async Task<IActionResult> Edit(EntityOptions opts)
        {

            // Get current user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // We need to be authenticated to edit
            if (user == null)
            {
                return Unauthorized();
            }

            // Get the entity
            var entity = await GetEntityAsync(opts.Id);

            // We don't have permission or the entity does not exist
            if (entity == null)
            {
                // Return a 404 if the entity does not exist
                // Return a 401 to indicate am authorization issue
                return await _entityStore.GetByIdAsync(opts.Id) == null
                    ? (IActionResult)NotFound()
                    : (IActionResult)Unauthorized();
            }

            // Do we have permission
            if (!await _authorizationService.AuthorizeAsync(this.User, entity.CategoryId,
                user?.Id == entity.CreatedUserId
                    ? Permissions.EditOwnArticles
                    : Permissions.EditAnyArticle))
            {
                return Unauthorized();
            }

            // Add entity we are editing to context
            HttpContext.Items[typeof(Article)] = entity;

            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                        .Action("Index", "Home", "Plato.Core")
                        .LocalNav()
                    ).Add(S["Articles"], index => index
                        .Action("Index", "Home", "Plato.Articles")
                        .LocalNav()
                    ).Add(S[entity.Title.TrimToAround(75)], display => display
                        .Action("Display", "Home", "Plato.Articles", new RouteValueDictionary()
                        {
                            ["opts.id"] = entity.Id,
                            ["opts.alias"] = entity.Alias
                        })
                        .LocalNav()
                    )
                    .Add(S["Edit Article"], post => post
                        .LocalNav()
                    );
            });

            // Return view
            return View((LayoutViewModel) await _entityViewProvider.ProvideEditAsync(entity, this));

        }

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Edit))]
        public async Task<IActionResult> EditPost(EditEntityViewModel viewModel)
        {

            // Get entity we are editing 
            var entity = await _entityStore.GetByIdAsync(viewModel.Id);

            // Ensure entity exists
            if (entity == null)
            {
                return NotFound();
            }

            // Get current user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // We always need to be logged in to edit entities
            if (user == null)
            {
                return Unauthorized();
            }

            // Do we have permission
            if (!await _authorizationService.AuthorizeAsync(this.User, entity.CategoryId,
                user?.Id == entity.CreatedUserId
                    ? Permissions.EditOwnArticles
                    : Permissions.EditAnyArticle))
            {
                return Unauthorized();
            }

            // Only update edited information if the message changes
            if (viewModel.Message != entity.Message)
            {
                entity.EditedUserId = user?.Id ?? 0;
                entity.EditedDate = DateTimeOffset.UtcNow;
            }

            //// Update title & message
            entity.Title = viewModel.Title;
            entity.Message = viewModel.Message;

            // Validate model state within all view providers
            if (await _entityViewProvider.IsModelStateValidAsync(entity, this))
            {

                // Always update modified information
                entity.ModifiedUserId = user?.Id ?? 0;
                entity.ModifiedDate = DateTimeOffset.UtcNow;

                // Get composed model from view providers
                entity = await _entityViewProvider.ComposeModelAsync(entity, this);

                // Update the entity
                var result = await _articleManager.UpdateAsync(entity);

                // Ensure success
                if (result.Succeeded)
                {

                    // Execute view providers ProvideUpdateAsync method
                    await _entityViewProvider.ProvideUpdateAsync(result.Response, this);

                    // Get authorize result
                    var authorizeResult = await AuthorizeAsync(result.Response);
                    if (authorizeResult.Succeeded)
                    {

                        // Everything was OK
                        _alerter.Success(T["Article Updated Successfully!"]);

                        // Redirect to entity
                        return RedirectToAction(nameof(Display), new RouteValueDictionary()
                        {
                            ["opts.id"] = entity.Id,
                            ["opts.alias"] = entity.Alias
                        });

                    }

                    // Add any authorization errors
                    foreach (var error in authorizeResult.Errors)
                    {
                        _alerter.Success(T[error.Description]);
                    }

                    // Redirect to index
                    return RedirectToAction(nameof(Index));

                }
                else
                {
                    // Errors that may have occurred whilst updating the entity
                    foreach (var error in result.Errors)
                    {
                        ViewData.ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                
            }
            
            return await Edit(new EntityOptions()
            {
                Id = entity.Id,
                Alias = entity.Alias
            });


        }
        
        // -----------------
        // Edit Reply
        // -----------------

        public async Task<IActionResult> EditReply(int id)
        {

            // Get reply we are editing
            var reply = await _entityReplyStore.GetByIdAsync(id);
            if (reply == null)
            {
                return NotFound();
            }

            // Get the entity
            var entity = await GetEntityAsync(reply.EntityId);

            // We don't have permission or the entity does not exist
            if (entity == null)
            {
                // Return a 404 if the entity does not exist
                // Return a 401 to indicate am authorization issue
                return await _entityStore.GetByIdAsync(reply.EntityId) == null
                    ? (IActionResult)NotFound()
                    : (IActionResult)Unauthorized();
            }

            // Get current user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // We always need to be logged in to edit entities
            if (user == null)
            {
                return Unauthorized();
            }

            // Do we have permission
            if (!await _authorizationService.AuthorizeAsync(this.User, entity.CategoryId,
                user?.Id == reply.CreatedUserId
                    ? Permissions.EditOwnArticleComment
                    : Permissions.EditAnyArticleComment))
            {
                return Unauthorized();
            }

            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                        .Action("Index", "Home", "Plato.Core")
                        .LocalNav()
                    ).Add(S["Articles"], articles => articles
                        .Action("Index", "Home", "Plato.Articles")
                        .LocalNav()
                    ).Add(S[entity.Title.TrimToAround(75)], post => post
                        .Action("Display", "Home", "Plato.Articles", new RouteValueDictionary()
                        {
                            ["opts.id"] = entity.Id,
                            ["opts.alias"] = entity.Alias
                        })
                        .LocalNav()
                    )
                    .Add(S["Edit Comment"], post => post
                        .LocalNav()
                    );
            });

            // Return view
            return View((LayoutViewModel) await _replyViewProvider.ProvideEditAsync(reply, this));

        }

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(EditReply))]
        public async Task<IActionResult> EditReplyPost(EditEntityReplyViewModel model)
        {

            // Ensure the reply exists
            var reply = await _entityReplyStore.GetByIdAsync(model.Id);
            if (reply == null)
            {
                return NotFound();
            }

            // Ensure the entity exists
            var entity = await _entityStore.GetByIdAsync(reply.EntityId);
            if (entity == null)
            {
                return NotFound();
            }

            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // We always need to be logged in to edit entities
            if (user == null)
            {
                return Unauthorized();
            }

            // Do we have permission
            if (!await _authorizationService.AuthorizeAsync(this.User, entity.CategoryId,
                user?.Id == reply.CreatedUserId
                    ? Permissions.EditOwnArticleComment
                    : Permissions.EditAnyArticleComment))
            {
                return Unauthorized();
            }

            // Only update edited information if the message changes
            if (model.Message != reply.Message)
            {
                reply.EditedUserId = user?.Id ?? 0;
                reply.EditedDate = DateTimeOffset.UtcNow;
            }

            // Always update modified date
            reply.ModifiedUserId = user?.Id ?? 0;
            reply.ModifiedDate = DateTimeOffset.UtcNow;

            // Update the message
            reply.Message = model.Message;

            // Validate model state within all view providers
            if (await _replyViewProvider.IsModelStateValidAsync(reply, this))
            {

                // Execute view providers ProvideUpdateAsync method
                await _replyViewProvider.ProvideUpdateAsync(reply, this);

                // Everything was OK
                _alerter.Success(T["Reply Updated Successfully!"]);

                // Redirect
                return RedirectToAction(nameof(Reply), new RouteValueDictionary()
                {
                    ["opts.id"] = entity.Id,
                    ["opts.alias"] = entity.Alias,
                    ["opts.replyId"] = reply.Id
                });

            }

            // if we reach this point some view model validation
            // failed within a view provider, display model state errors
            foreach (var modelState in ViewData.ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    _alerter.Danger(T[error.ErrorMessage]);
                }
            }

            return await Create(0);

        }
        
        // -----------------
        // Report Entity
        // -----------------

        public Task<IActionResult> Report(EntityOptions opts)
        {

            if (opts == null)
            {
                opts = new EntityOptions();
            }

            var viewModel = new ReportEntityViewModel()
            {
                Options = opts,
                AvailableReportReasons = GetReportReasons()
            };

            // Return view
            return Task.FromResult((IActionResult)View(viewModel));

        }
        
        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Report))]
        public async Task<IActionResult> ReportPost(ReportEntityViewModel model)
        {

            // Ensure the entity exists
            var entity = await _entityStore.GetByIdAsync(model.Options.Id);
            if (entity == null)
            {
                return NotFound();
            }

            // Ensure the reply exists
            Comment reply = null;
            if (model.Options.ReplyId > 0)
            {
                reply = await _entityReplyStore.GetByIdAsync(model.Options.ReplyId);
                if (reply == null)
                {
                    return NotFound();
                }
            }

            // Get authenticated user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Invoke report manager and compile results
            if (reply != null)
            {
                // Report reply
                await _reportReplyManager.ReportAsync(new ReportSubmission<Comment>()
                {
                    Who = user,
                    What = reply,
                    Why = (ReportReasons.Reason)model.ReportReason
                });
            }
            else
            {
                // Report entity
                await _reportEntityManager.ReportAsync(new ReportSubmission<Article>()
                {
                    Who = user,
                    What = entity,
                    Why = (ReportReasons.Reason)model.ReportReason
                });
            }

            _alerter.Success(reply != null
                ? T["Thank You. Comment Reported Successfully!"]
                : T["Thank You. Article Reported Successfully!"]);

            // Redirect
            return RedirectToAction(nameof(Reply), new RouteValueDictionary()
            {
                ["opts.id"] = entity.Id,
                ["opts.alias"] = entity.Alias,
                ["opts.replyId"] = reply?.Id ?? 0
            });

        }

        // -----------------
        //  Display Reply
        // -----------------

        public async Task<IActionResult> Reply(EntityOptions opts)
        {
            
            // Default options
            if (opts == null)
            {
                opts = new EntityOptions();
            }
            
            // Get entity
            var entity = await _entityStore.GetByIdAsync(opts.Id);

            // Ensure entity exists
            if (entity == null)
            {
                return NotFound();
            }

            // Configure options
            opts = ConfigureEntityDisplayOptions(entity, opts);

            // Get offset for given reply
            var offset = 0;
            if (opts.ReplyId > 0)
            {
                // We need to iterate all replies to calculate the offset
                var replies = await _replyService
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
                    .GetResultsAsync(opts, new PagerOptions
                {
                    Size = int.MaxValue
                });
                if (replies?.Data != null)
                {
                    foreach (var reply in replies.Data)
                    {
                        offset++;
                        if (reply.Id == opts.ReplyId)
                        {
                            break;
                        }
                    }
                }
            }

            if (offset == 0)
            {
                // Could not locate offset, fallback by redirecting to entity
                return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
                {
                    ["area"] = "Plato.Articles",
                    ["controller"] = "Home",
                    ["action"] = "Display",
                    ["opts.id"] = entity.Id,
                    ["opts.alias"] = entity.Alias
                }));
            }

            // Redirect to offset within entity
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Articles",
                ["controller"] = "Home",
                ["action"] = "Display",
                ["opts.id"] = entity.Id,
                ["opts.alias"] = entity.Alias,
                ["pager.offset"] = offset
            }));

        }

        // -----------------
        // Entity Helpers
        // -----------------

        public async Task<IActionResult> Pin(string id)
        {

            // Ensure we have a valid id
            var ok = int.TryParse(id, out var entityId);
            if (!ok)
            {
                return NotFound();
            }

            var entity = await _entityStore.GetByIdAsync(entityId);

            // Ensure the entity exists
            if (entity == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, entity.CategoryId, Permissions.PinArticles))
            {
                return Unauthorized();
            }

            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Update entity
            entity.ModifiedUserId = user?.Id ?? 0;
            entity.ModifiedDate = DateTimeOffset.UtcNow;
            entity.IsPinned = true;

            // Save changes and return results
            var result = await _articleManager.UpdateAsync(entity);

            if (result.Succeeded)
            {
                _alerter.Success(T["Pinned Successfully"]);
            }
            else
            {
                _alerter.Danger(T["Could not pin the article"]);
            }

            // Redirect back to entity
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Articles",
                ["controller"] = "Home",
                ["action"] = "Display",
                ["opts.id"] = entity.Id,
                ["opts.alias"] = entity.Alias
            }));

        }

        public async Task<IActionResult> Unpin(string id)
        {

            // Ensure we have a valid id
            var ok = int.TryParse(id, out var entityId);
            if (!ok)
            {
                return NotFound();
            }

            var entity = await _entityStore.GetByIdAsync(entityId);

            // Ensure the entity exists
            if (entity == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, entity.CategoryId, Permissions.UnpinArticles))
            {
                return Unauthorized();
            }

            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Update entity
            entity.ModifiedUserId = user?.Id ?? 0;
            entity.ModifiedDate = DateTimeOffset.UtcNow;
            entity.IsPinned = false;

            // Save changes and return results
            var result = await _articleManager.UpdateAsync(entity);

            if (result.Succeeded)
            {
                _alerter.Success(T["Pin Removed Successfully"]);
            }
            else
            {
                _alerter.Danger(T["Could not remove pin"]);
            }

            // Redirect back to entity
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Articles",
                ["controller"] = "Home",
                ["action"] = "Display",
                ["opts.id"] = entity.Id,
                ["opts.alias"] = entity.Alias
            }));

        }

        public async Task<IActionResult> Hide(string id)
        {

            // Ensure we have a valid id
            var ok = int.TryParse(id, out int entityId);
            if (!ok)
            {
                return NotFound();
            }

            var entity = await _entityStore.GetByIdAsync(entityId);

            // Ensure the entity exists
            if (entity == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, entity.CategoryId, Permissions.HideArticles))
            {
                return Unauthorized();
            }

            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Update entity
            entity.ModifiedUserId = user?.Id ?? 0;
            entity.ModifiedDate = DateTimeOffset.UtcNow;
            entity.IsHidden = true;

            // Save changes and return results
            var result = await _articleManager.UpdateAsync(entity);

            if (result.Succeeded)
            {
                _alerter.Success(T["Article Hidden Successfully"]);
                
                if (result.Response.IsHidden)
                {
                    // Do we have permission to view hidden entities
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        entity.CategoryId, Permissions.ViewHiddenArticles))
                    {
                        // Redirect to index
                        return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
                        {
                            ["area"] = "Plato.Articles",
                            ["controller"] = "Home",
                            ["action"] = "Index"
                        }));
                    }
                }
                
            }
            else
            {
                _alerter.Danger(T["Could not hide the article"]);
            }

            // Redirect back to entity
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Articles",
                ["controller"] = "Home",
                ["action"] = "Display",
                ["opts.id"] = entity.Id,
                ["opts.alias"] = entity.Alias
            }));

        }

        public async Task<IActionResult> Show(string id)
        {

            // Ensure we have a valid id
            var ok = int.TryParse(id, out var entityId);
            if (!ok)
            {
                return NotFound();
            }

            var entity = await _entityStore.GetByIdAsync(entityId);

            // Ensure the entity exists
            if (entity == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, entity.CategoryId, Permissions.ShowArticles))
            {
                return Unauthorized();
            }

            // Get authenticated user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Update entity
            entity.ModifiedUserId = user?.Id ?? 0;
            entity.ModifiedDate = DateTimeOffset.UtcNow;
            entity.IsHidden = false;

            // Save changes and return results
            var result = await _articleManager.UpdateAsync(entity);

            if (result.Succeeded)
            {
                _alerter.Success(T["Article Made Public Successfully"]);
            }
            else
            {
                _alerter.Danger(T["Could not update the article"]);
            }

            // Redirect back to entity
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Articles",
                ["controller"] = "Home",
                ["action"] = "Display",
                ["opts.id"] = entity.Id,
                ["opts.alias"] = entity.Alias
            }));

        }

        public async Task<IActionResult> Lock(string id)
        {

            // Ensure we have a valid id
            var ok = int.TryParse(id, out int entityId);
            if (!ok)
            {
                return NotFound();
            }

            var entity = await _entityStore.GetByIdAsync(entityId);

            // Ensure the entity exists
            if (entity == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, entity.CategoryId, Permissions.LockArticles))
            {
                return Unauthorized();
            }

            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Update entity
            entity.ModifiedUserId = user?.Id ?? 0;
            entity.ModifiedDate = DateTimeOffset.UtcNow;
            entity.IsLocked = true;

            // Save changes and return results
            var result = await _articleManager.UpdateAsync(entity);

            if (result.Succeeded)
            {
                _alerter.Success(T["Article Locked Successfully"]);
            }
            else
            {
                _alerter.Danger(T["Could not lock the article"]);
            }

            // Redirect back to entity
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Articles",
                ["controller"] = "Home",
                ["action"] = "Display",
                ["opts.id"] = entity.Id,
                ["opts.alias"] = entity.Alias
            }));

        }

        public async Task<IActionResult> Unlock(string id)
        {

            // Ensure we have a valid id
            var ok = int.TryParse(id, out var entityId);
            if (!ok)
            {
                return NotFound();
            }

            var entity = await _entityStore.GetByIdAsync(entityId);

            // Ensure the entity exists
            if (entity == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, entity.CategoryId, Permissions.UnlockArticles))
            {
                return Unauthorized();
            }

            // Get authenticated user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Update entity
            entity.ModifiedUserId = user?.Id ?? 0;
            entity.ModifiedDate = DateTimeOffset.UtcNow;
            entity.IsLocked = false;

            // Save changes and return results
            var result = await _articleManager.UpdateAsync(entity);

            if (result.Succeeded)
            {
                _alerter.Success(T["Article Unlocked Successfully"]);
            }
            else
            {
                _alerter.Danger(T["Could not unlock the article"]);
            }

            // Redirect back to entity
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Articles",
                ["controller"] = "Home",
                ["action"] = "Display",
                ["opts.id"] = entity.Id,
                ["opts.alias"] = entity.Alias
            }));

        }

        public async Task<IActionResult> ToSpam(string id)
        {

            // Ensure we have a valid id
            var ok = int.TryParse(id, out int entityId);
            if (!ok)
            {
                return NotFound();
            }

            var entity = await _entityStore.GetByIdAsync(entityId);

            // Ensure the entity exists
            if (entity == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, entity.CategoryId, Permissions.ArticleToSpam))
            {
                return Unauthorized();
            }

            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Update entity
            entity.ModifiedUserId = user?.Id ?? 0;
            entity.ModifiedDate = DateTimeOffset.UtcNow;
            entity.IsSpam = true;

            // Save changes and return results
            var result = await _articleManager.UpdateAsync(entity);

            if (result.Succeeded)
            {
                _alerter.Success(T["Article Marked as SPAM"]);

                if (result.Response.IsSpam)
                {
                    // Do we have permission to view spam entities
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        entity.CategoryId, Permissions.ViewSpamArticles))
                    {
                        // Redirect to index
                        return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
                        {
                            ["area"] = "Plato.Articles",
                            ["controller"] = "Home",
                            ["action"] = "Index"
                        }));
                    }
                }
                
            }
            else
            {
                _alerter.Danger(T["Could not mark article as SPAM"]);
            }

            // Redirect back to entity
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Articles",
                ["controller"] = "Home",
                ["action"] = "Display",
                ["opts.id"] = entity.Id,
                ["opts.alias"] = entity.Alias
            }));

        }

        public async Task<IActionResult> FromSpam(string id)
        {

            // Ensure we have a valid id
            var ok = int.TryParse(id, out var entityId);
            if (!ok)
            {
                return NotFound();
            }

            var entity = await _entityStore.GetByIdAsync(entityId);

            // Ensure the entity exists
            if (entity == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, entity.CategoryId, Permissions.ArticleFromSpam))
            {
                return Unauthorized();
            }

            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Update entity
            entity.ModifiedUserId = user?.Id ?? 0;
            entity.ModifiedDate = DateTimeOffset.UtcNow;
            entity.IsSpam = false;

            // Save changes and return results
            var result = await _articleManager.UpdateAsync(entity);

            if (result.Succeeded)
            {
                _alerter.Success(T["Article Removed from SPAM"]);
            }
            else
            {
                _alerter.Danger(T["Could not remove the article from SPAM"]);
            }

            // Redirect back to entity
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Articles",
                ["controller"] = "Home",
                ["action"] = "Display",
                ["opts.id"] = entity.Id,
                ["opts.alias"] = entity.Alias
            }));

        }

        public async Task<IActionResult> Delete(string id)
        {

            // Ensure we have a valid id
            var ok = int.TryParse(id, out var entityId);
            if (!ok)
            {
                return NotFound();
            }

            // Get current user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Ensure we are authenticated
            if (user == null)
            {
                return Unauthorized();
            }

            // Get entity
            var entity = await _entityStore.GetByIdAsync(entityId);

            // Ensure the entity exists
            if (entity == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(this.User, entity.CategoryId,
                user.Id == entity.CreatedUserId
                    ? Permissions.DeleteOwnArticles
                    : Permissions.DeleteAnyArticle))
            {
                return Unauthorized();
            }

            // Update entity
            entity.ModifiedUserId = user?.Id ?? 0;
            entity.ModifiedDate = DateTimeOffset.UtcNow;
            entity.IsDeleted = true;

            // Save changes and return results
            var result = await _articleManager.UpdateAsync(entity);
            if (result.Succeeded)
            {
                _alerter.Success(T["Article Deleted Successfully"]);

                if (result.Response.IsDeleted)
                {
                    // Do we have permission to view deleted entities
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        entity.CategoryId, Permissions.ViewDeletedArticles))
                    {
                        // Redirect to index
                        return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
                        {
                            ["area"] = "Plato.Articles",
                            ["controller"] = "Home",
                            ["action"] = "Index"
                        }));
                    }
                }
             
            }
            else
            {
                _alerter.Danger(T["Could not delete the article"]);
            }

            // Redirect back to entity
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Articles",
                ["controller"] = "Home",
                ["action"] = "Display",
                ["opts.id"] = entity.Id,
                ["opts.alias"] = entity.Alias
            }));

        }

        public async Task<IActionResult> Restore(string id)
        {

            // Ensure we have a valid id
            var ok = int.TryParse(id, out var entityId);
            if (!ok)
            {
                return NotFound();
            }

            // Get current user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Ensure we are authenticated
            if (user == null)
            {
                return Unauthorized();
            }

            // Get entity
            var entity = await _entityStore.GetByIdAsync(entityId);

            // Ensure the entity exists
            if (entity == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(this.User, entity.CategoryId,
                user.Id == entity.CreatedUserId
                    ? Permissions.RestoreOwnArticles
                    : Permissions.RestoreAnyArticle))
            {
                return Unauthorized();
            }

            // Update entity
            entity.ModifiedUserId = user?.Id ?? 0;
            entity.ModifiedDate = DateTimeOffset.UtcNow;
            entity.IsDeleted = false;

            // Save changes and return results
            var result = await _articleManager.UpdateAsync(entity);
            if (result.Succeeded)
            {
                _alerter.Success(T["Article Restored Successfully"]);
            }
            else
            {
                _alerter.Danger(T["Could not restore the article"]);
            }

            // Redirect back to entity
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Articles",
                ["controller"] = "Home",
                ["action"] = "Display",
                ["opts.id"] = entity.Id,
                ["opts.alias"] = entity.Alias
            }));

        }

        public async Task<IActionResult> PermanentDelete(string id)
        {

            // Ensure we have a valid id
            var ok = int.TryParse(id, out var entityId);
            if (!ok)
            {
                return NotFound();
            }

            // Get current user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Ensure we are authenticated
            if (user == null)
            {
                return Unauthorized();
            }

            // Get entity
            var entity = await _entityStore.GetByIdAsync(entityId);

            // Ensure the entity exists
            if (entity == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(this.User, entity.CategoryId,
                user.Id == entity.CreatedUserId
                    ? Permissions.PermanentDeleteOwnArticles
                    : Permissions.PermanentDeleteAnyArticle))
            {
                return Unauthorized();
            }

            // Delete entity
            var result = await _articleManager.DeleteAsync(entity);
            if (result.Succeeded)
            {
                _alerter.Success(T["Article Permanently Deleted Successfully"]);
            }
            else
            {
                _alerter.Danger(T["Could not permanently delete the article"]);
            }

            // Redirect back to index
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Articles",
                ["controller"] = "Home",
                ["action"] = "Index"
            }));

        }

        // -----------------
        // Entity Reply Helpers
        // -----------------

        public async Task<IActionResult> HideReply(string id)
        {

            // Ensure we have a valid id
            var ok = int.TryParse(id, out var replyId);
            if (!ok)
            {
                return NotFound();
            }

            var reply = await _entityReplyStore.GetByIdAsync(replyId);
            if (reply == null)
            {
                return NotFound();
            }

            var entity = await _entityStore.GetByIdAsync(reply.EntityId);

            // Ensure the entity exists
            if (entity == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, entity.CategoryId, Permissions.HideArticleComments))
            {
                return Unauthorized();
            }

            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Update entity
            reply.ModifiedUserId = user?.Id ?? 0;
            reply.ModifiedDate = DateTimeOffset.UtcNow;
            reply.IsHidden = true;

            // Save changes and return results
            var result = await _commentManager.UpdateAsync(reply);

            if (result.Succeeded)
            {
                _alerter.Success(T["Comment Hidden Successfully"]);
            }
            else
            {
                _alerter.Danger(T["Could not hide the comment"]);
            }

            // Redirect back to reply
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Articles",
                ["controller"] = "Home",
                ["action"] = "Reply",
                ["opts.id"] = entity.Id,
                ["opts.alias"] = entity.Alias,
                ["opts.replyId"] = reply.Id
            }));

        }

        public async Task<IActionResult> ShowReply(string id)
        {

            // Ensure we have a valid id
            var ok = int.TryParse(id, out var replyId);
            if (!ok)
            {
                return NotFound();
            }

            var reply = await _entityReplyStore.GetByIdAsync(replyId);

            if (reply == null)
            {
                return NotFound();
            }

            var entity = await _entityStore.GetByIdAsync(reply.EntityId);

            // Ensure the entity exists
            if (entity == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, entity.CategoryId, Permissions.ShowArticleComments))
            {
                return Unauthorized();
            }

            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Update entity
            reply.ModifiedUserId = user?.Id ?? 0;
            reply.ModifiedDate = DateTimeOffset.UtcNow;
            reply.IsHidden = false;

            // Save changes and return results
            var result = await _commentManager.UpdateAsync(reply);

            if (result.Succeeded)
            {
                _alerter.Success(T["Comment Made Public Successfully"]);
            }
            else
            {
                _alerter.Danger(T["Could not make the comment public"]);
            }
            // Redirect back to reply
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Articles",
                ["controller"] = "Home",
                ["action"] = "Reply",
                ["opts.id"] = entity.Id,
                ["opts.alias"] = entity.Alias,
                ["opts.replyId"] = reply.Id
            }));


        }

        public async Task<IActionResult> ReplyToSpam(string id)
        {

            // Ensure we have a valid id
            var ok = int.TryParse(id, out var replyId);
            if (!ok)
            {
                return NotFound();
            }

            var reply = await _entityReplyStore.GetByIdAsync(replyId);

            if (reply == null)
            {
                return NotFound();
            }

            var entity = await _entityStore.GetByIdAsync(reply.EntityId);

            // Ensure the entity exists
            if (entity == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, entity.CategoryId, Permissions.ArticleCommentToSpam))
            {
                return Unauthorized();
            }

            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Update entity
            reply.ModifiedUserId = user?.Id ?? 0;
            reply.ModifiedDate = DateTimeOffset.UtcNow;
            reply.IsSpam = true;

            // Save changes and return results
            var result = await _commentManager.UpdateAsync(reply);

            if (result.Succeeded)
            {
                _alerter.Success(T["Comment Marked as SPAM"]);
            }
            else
            {
                _alerter.Danger(T["Could not mark the comment as SPAM"]);
            }

            // Redirect back to reply
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Articles",
                ["controller"] = "Home",
                ["action"] = "Reply",
                ["opts.id"] = entity.Id,
                ["opts.alias"] = entity.Alias,
                ["opts.replyId"] = reply.Id
            }));


        }

        public async Task<IActionResult> ReplyFromSpam(string id)
        {

            // Ensure we have a valid id
            var ok = int.TryParse(id, out var replyId);
            if (!ok)
            {
                return NotFound();
            }

            var reply = await _entityReplyStore.GetByIdAsync(replyId);

            if (reply == null)
            {
                return NotFound();
            }

            var entity = await _entityStore.GetByIdAsync(reply.EntityId);

            // Ensure the entity exists
            if (entity == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, entity.CategoryId, Permissions.ArticleCommentFromSpam))
            {
                return Unauthorized();
            }

            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Update entity
            reply.ModifiedUserId = user?.Id ?? 0;
            reply.ModifiedDate = DateTimeOffset.UtcNow;
            reply.IsSpam = false;

            // Save changes and return results
            var result = await _commentManager.UpdateAsync(reply);

            if (result.Succeeded)
            {
                _alerter.Success(T["Comment Removed from SPAM"]);
            }
            else
            {
                _alerter.Danger(T["Could not remove the comment from SPAM"]);
            }

            // Redirect back to reply
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Articles",
                ["controller"] = "Home",
                ["action"] = "Reply",
                ["opts.id"] = entity.Id,
                ["opts.alias"] = entity.Alias,
                ["opts.replyId"] = reply.Id
            }));
            
        }

        public async Task<IActionResult> DeleteReply(string id)
        {

            // Ensure we have a valid id
            var ok = int.TryParse(id, out var replyId);
            if (!ok)
            {
                return NotFound();
            }

            // Get current user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Ensure we are authenticated
            if (user == null)
            {
                return Unauthorized();
            }

            // Ensure the reply exists
            var reply = await _entityReplyStore.GetByIdAsync(replyId);
            if (reply == null)
            {
                return NotFound();
            }

            // Ensure the entity exists
            var entity = await _entityStore.GetByIdAsync(reply.EntityId);
            if (entity == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(this.User, entity.CategoryId,
                user.Id == reply.CreatedUserId
                    ? Permissions.DeleteOwnArticleComments
                    : Permissions.DeleteAnyArticleComment))
            {
                return Unauthorized();
            }

            // Update reply
            reply.ModifiedUserId = user?.Id ?? 0;
            reply.ModifiedDate = DateTimeOffset.UtcNow;
            reply.IsDeleted = true;

            // Save changes and return results
            var result = await _commentManager.UpdateAsync(reply);

            if (result.Succeeded)
            {
                _alerter.Success(T["Comment Deleted Successfully"]);
            }
            else
            {
                _alerter.Danger(T["Could not delete the comment"]);
            }

            // Redirect back to entity
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Articles",
                ["controller"] = "Home",
                ["action"] = "Reply",
                ["opts.id"] = entity.Id,
                ["opts.alias"] = entity.Alias,
                ["opts.replyId"] = reply.Id
            }));

        }

        public async Task<IActionResult> RestoreReply(string id)
        {

            // Ensure we have a valid id
            var ok = int.TryParse(id, out var replyId);
            if (!ok)
            {
                return NotFound();
            }

            // Get current user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Ensure we are authenticated
            if (user == null)
            {
                return Unauthorized();
            }

            // Ensure the reply exists
            var reply = await _entityReplyStore.GetByIdAsync(replyId);
            if (reply == null)
            {
                return NotFound();
            }

            // Ensure the entity exists
            var entity = await _entityStore.GetByIdAsync(reply.EntityId);
            if (entity == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(this.User, entity.CategoryId,
                user.Id == reply.CreatedUserId
                    ? Permissions.RestoreOwnArticleComments
                    : Permissions.RestoreAnyArticleComment))
            {
                return Unauthorized();
            }

            // Update reply
            reply.ModifiedUserId = user?.Id ?? 0;
            reply.ModifiedDate = DateTimeOffset.UtcNow;
            reply.IsDeleted = false;

            // Save changes and return results
            var result = await _commentManager.UpdateAsync(reply);

            if (result.Succeeded)
            {
                _alerter.Success(T["Comment Restored Successfully"]);
            }
            else
            {
                _alerter.Danger(T["Could not restore the comment"]);
            }

            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Articles",
                ["controller"] = "Home",
                ["action"] = "Reply",
                ["opts.id"] = entity.Id,
                ["opts.alias"] = entity.Alias,
                ["opts.replyId"] = reply.Id
            }));

        }

        public async Task<IActionResult> PermanentDeleteReply(string id)
        {

            // Ensure we have a valid id
            var ok = int.TryParse(id, out var replyId);
            if (!ok)
            {
                return NotFound();
            }

            // Get current user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Ensure we are authenticated
            if (user == null)
            {
                return Unauthorized();
            }

            // Ensure the reply exists
            var reply = await _entityReplyStore.GetByIdAsync(replyId);
            if (reply == null)
            {
                return NotFound();
            }

            // Ensure the entity exists
            var entity = await _entityStore.GetByIdAsync(reply.EntityId);
            if (entity == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(this.User, entity.CategoryId,
                user.Id == reply.CreatedUserId
                    ? Permissions.PermanentDeleteOwnArticleComments
                    : Permissions.PermanentDeleteAnyArticleComment))
            {
                return Unauthorized();
            }

            // Delete reply
            var result = await _commentManager.DeleteAsync(reply);
            if (result.Succeeded)
            {
                _alerter.Success(T["Comment Permanently Deleted Successfully"]);
            }
            else
            {
                _alerter.Danger(T["Could not permanently delete the comment"]);
            }

            // Redirect back to entity
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Articles",
                ["controller"] = "Home",
                ["action"] = "Display",
                ["opts.id"] = entity.Id,
                ["opts.alias"] = entity.Alias
            }));

        }

        #endregion

        #region "Private Methods"

        // Use the entity service to get the entity to 
        // ensure query adapters are enforced
        async Task<Article> GetEntityAsync(int entityId)
        {

            if (entityId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(entityId));
            }

            return await _articleService
                .ConfigureQuery(async q =>
                {

                    // Get entity
                    q.Id.Equals(entityId);

                    // Hide private?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewPrivateArticles))
                    {
                        q.HidePrivate.True();
                    }

                    // Hide hidden?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewHiddenArticles))
                    {
                        q.HideHidden.True();
                    }

                    // Hide spam?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewSpamArticles))
                    {
                        q.HideSpam.True();
                    }

                    // Hide deleted?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewDeletedArticles))
                    {
                        q.HideDeleted.True();
                    }

                })
                .GetResultAsync();

        }

        async Task<ICommandResultBase> AuthorizeAsync(IEntity entity)
        {

            // Our result
            var result = new CommandResultBase();

            // Generic error message
            const string error = "Article added but pending approval";

            // IsHidden
            if (entity.IsHidden)
            {
                if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                    entity.CategoryId, Permissions.ViewHiddenArticles))
                {
                    return result.Failed(error);
                }
            }

            // IsSpam
            if (entity.IsSpam)
            {
                if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                    entity.CategoryId, Permissions.ViewSpamArticles))
                {
                    return result.Failed(error);
                }
            }

            // IsDeleted
            if (entity.IsDeleted)
            {
                if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                    entity.CategoryId, Permissions.ViewDeletedArticles))
                {
                    return result.Failed(error);
                }
            }

            return result.Success();

        }

        async Task<ICommandResultBase> AuthorizeAsync(IEntityReply reply)
        {

            // Our result
            var result = new CommandResultBase();

            // Get entity
            var entity = await _entityStore.GetByIdAsync(reply.EntityId);

            // Ensure entity exists
            if (entity == null)
            {
                return result.Failed("The article has since been deleted!");
            }

            // Generic failure message
            const string error = "Comment added but pending approval";

            // IsHidden
            if (reply.IsHidden)
            {
                if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                    entity.CategoryId, Permissions.ViewHiddenArticleComments))
                {
                    return result.Failed(error);
                }
            }

            // IsSpam
            if (reply.IsSpam)
            {
                if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                    entity.CategoryId, Permissions.ViewSpamArticleComments))
                {
                    return result.Failed(error);
                }
            }

            // IsDeleted
            if (reply.IsDeleted)
            {
                if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                    entity.CategoryId, Permissions.ViewDeletedArticleComments))
                {
                    return result.Failed(error);
                }
            }

            return result.Success();

        }
        
        async Task<EntityIndexViewModel<Article>> GetIndexViewModelAsync(EntityIndexOptions options, PagerOptions pager)
        {

            // Get current feature
            var feature = await _featureFacade.GetFeatureByIdAsync(RouteData.Values["area"].ToString());

            // Restrict results to current feature
            if (feature != null)
            {
                options.FeatureId = feature.Id;
            }

            // Set pager call back Url
            pager.Url = _contextFacade.GetRouteUrl(pager.Route(RouteData));

            // Ensure pinned appear first
            if (options.Sort == SortBy.LastReply)
            {
                options.AddSortColumn(SortBy.IsPinned.ToString(), OrderBy.Desc);
            }

            // Return updated model
            return new EntityIndexViewModel<Article>()
            {
                Options = options,
                Pager = pager
            };

        }
        
        EntityViewModel<Article, Comment> GetDisplayViewModel(Article entity, EntityOptions options, PagerOptions pager)
        {

            // Set pager call back Url
            pager.Url = _contextFacade.GetRouteUrl(pager.Route(RouteData));

            // Configure options
            options = ConfigureEntityDisplayOptions(entity, options);

            // Return updated view model
            return new EntityViewModel<Article, Comment>()
            {
                Entity = entity,
                Options = options,
                Pager = pager
            };
        }
        
        EntityOptions ConfigureEntityDisplayOptions(Article entity, EntityOptions options)
        {

            // Ensure view model is aware of the entity we are displaying
            options.Id = entity.Id;

            // Ensure replies marked as an answer appear first
            options.SortColumns.Add("IsAnswer", OrderBy.Desc);
            options.SortColumns.Add("CreatedDate", OrderBy.Asc);

            return options;

        }

        IEnumerable<SelectListItem> GetReportReasons()
        {

            var output = new List<SelectListItem>();
            foreach (var reason in ReportReasons.Reasons)
            {
                output.Add(new SelectListItem
                {
                    Text = S[reason.Value],
                    Value = Convert.ToString((int)reason.Key)
                });
            }

            return output;
        }

        #endregion

    }

}
