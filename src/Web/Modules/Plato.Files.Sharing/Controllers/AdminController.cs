using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Files.Models;
using Plato.Files.Sharing.Models;
using Plato.Files.Sharing.Services;
using Plato.Files.Sharing.Stores;
using Plato.Files.Sharing.ViewModels;
using Plato.Files.Stores;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.Alerts;
using PlatoCore.Layout.ModelBinding;

namespace Plato.Files.Sharing.Controllers
{
    public class AdminController : Controller, IUpdateModel
    {

        private readonly IFileInviteStore<FileInvite> _fileInviteStore;
        private readonly IEmailFileInviteService _shareInviteService;
        private readonly IContextFacade _contextFacade;
        private readonly IFileStore<File> _fileStore;
        

        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public AdminController(

            IHtmlLocalizer<AdminController> htmlLocalizer,
            IStringLocalizer<AdminController> stringLocalizer,
            IFileInviteStore<FileInvite> fileInviteStore,
            IEmailFileInviteService shareInviteService,
            IContextFacade contextFacade,
            IFileStore<File> fileStore,            
            IAlerter alerter)
        {

            _shareInviteService = shareInviteService;
            _fileInviteStore = fileInviteStore;
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

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ShareFileAttachment(ShareFileViewModel model)
        {

            var email = model.AttachmentEmail.Trim();

            // Ensure we have an email to share with
            if (string.IsNullOrEmpty(email))
            {

                // Add alert
                _alerter.Danger(T["An email address is required!"]);

                // Redirect back to file
                return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
                {
                    ["area"] = "Plato.Files",
                    ["controller"] = "Admin",
                    ["action"] = "Edit",
                    ["id"] = model.FileId
                }));

            }

            // Get current user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // We need to be authenticated to add the invite
            if (user == null)
            {
                return Unauthorized();
            }

            // Create the invite
            var invite = await _fileInviteStore.CreateAsync(new FileInvite()
            {
                FileId = model.FileId,
                Email = email,
                CreatedUserId = user.Id,
                CreatedDate = DateTimeOffset.Now
            });

            // Share the invite
            if (invite != null)
            {
                var result = await _shareInviteService.SendAttachmentInviteAsync(invite);
                if (result.Succeeded)
                {
                    _alerter.Success(T["File Shared Successfully!"]);
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        if (!string.IsNullOrEmpty(error.Description))
                        {
                            _alerter.Danger(T[error.Description]);
                        }                        
                    }
                }
            }

            // Redirect back to file
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Files",
                ["controller"] = "Admin",
                ["action"] = "Edit",
                ["id"] = model.FileId
            }));

        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ShareFileLink(ShareFileViewModel model)
        {

            var email = model.LinkEmail.Trim();

            // Ensure we have an email to share with
            if (string.IsNullOrEmpty(email))
            {
                // Add alert
                _alerter.Danger(T["An email address is required!"]);

                // Redirect back to file
                return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
                {
                    ["area"] = "Plato.Files",
                    ["controller"] = "Admin",
                    ["action"] = "Edit",
                    ["id"] = model.FileId
                }));

            }

            // Get current user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // We need to be authenticated to add the invite
            if (user == null)
            {
                return Unauthorized();
            }
                        
            // Create the invite
            var invite = await _fileInviteStore.CreateAsync(new FileInvite()
            {
                FileId = model.FileId,
                Email = email,
                CreatedUserId = user.Id,
                CreatedDate = DateTimeOffset.Now
            });

            // Share the invite
            if (invite != null)
            {
                var result = await _shareInviteService.SendLinkInviteAsync(invite);
                if (result.Succeeded)
                {
                    _alerter.Success(T["File Shared Successfully!"]);
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        _alerter.Danger(T[error.Description]);
                    }
                }
            }

            // Redirect back to file
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
