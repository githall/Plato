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
        
        private readonly IContextFacade _contextFacade;
        private readonly IFileStore<File> _fileStore;
        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public AdminController(
            IHtmlLocalizer<AdminController> htmlLocalizer,
            IStringLocalizer<AdminController> stringLocalizer,
            IContextFacade contextFacade,
            IFileStore<File> fileStore,            
            IAlerter alerter)
        {
            
            _contextFacade = contextFacade;
            _fileStore = fileStore;
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

            // Get file
            var file = await _fileStore.GetByIdAsync(id);

            // Ensure the file exists
            if (file == null)
            {
                return NotFound();
            }

            // Return view
            return View(new ShareFileViewModel
            {
                FileId = file.Id,
                File = file
            });

        }

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Index))]
        public IActionResult IndexPost(ShareFileViewModel model)
        {

            // Add alert
            _alerter.Success(T["File Shared Successfully!"]);     

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
