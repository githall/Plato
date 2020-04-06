using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Files.Models;
using Plato.Files.Sharing.ViewModels;
using Plato.Files.Stores;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.Alerts;
using PlatoCore.Layout.ModelBinding;

namespace Plato.Files.Sharing.Controllers
{
    public class AdminController : Controller, IUpdateModel
    {

        private readonly IFileStore<File> _entityStore;
        private readonly IContextFacade _contextFacade;
        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public AdminController(
            IHtmlLocalizer<AdminController> htmlLocalizer,
            IStringLocalizer<AdminController> stringLocalizer,
            IFileStore<File> entityStore,
            IContextFacade contextFacade,
            IAlerter alerter)
        {

            _entityStore = entityStore;
            _contextFacade = contextFacade;
            _alerter = alerter;

            T = htmlLocalizer;
            S = stringLocalizer;

        }

        // Share dialog

        public async Task<IActionResult> Index(int id)
        {

            // We always need an file Id
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            // Build route values
            RouteValueDictionary routeValues = null;

            //// Append offset
            //if (opts.ReplyId > 0)
            //{
            //    routeValues = new RouteValueDictionary()
            //    {
            //        ["area"] = "Plato.Docs",
            //        ["controller"] = "Home",
            //        ["action"] = "Reply",
            //        ["opts.id"] = entity.Id,
            //        ["opts.alias"] = entity.Alias,
            //        ["opts.replyId"] = opts.ReplyId
            //    };
            //}
            //else
            //{
            //    routeValues = new RouteValueDictionary()
            //    {
            //        ["area"] = "Plato.Docs",
            //        ["controller"] = "Home",
            //        ["action"] = "Display",
            //        ["opts.id"] = entity.Id,
            //        ["opts.alias"] = entity.Alias
            //    };
            //}

            // Build view model
            var baseUrl = await _contextFacade.GetBaseUrlAsync();
            var viewModel = new ShareFileViewModel
            {
                FileId = id             
            };

            // Return view
            return View(viewModel);

        }

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Index))]
        public IActionResult IndexPost(ShareFileViewModel model)
        {

            // Add alert
            _alerter.Success(T["Settings Updated Successfully!"]);     

            // Redirect to offset within entity
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Files",
                ["controller"] = "Admin",
                ["action"] = "Edit",
                ["id"] = model.FileId
            }));

        }

    }

}
