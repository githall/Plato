using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Net.Http.Headers;
using Plato.Files.Models;
using Plato.Files.Services;
using Plato.Files.Sharing.Models;
using Plato.Files.Sharing.Services;
using Plato.Files.Sharing.Stores;
using Plato.Files.Stores;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Hosting.Web.Abstractions;
using PlatoCore.Hosting.Web.Abstractions;
using PlatoCore.Layout.Alerts;
using PlatoCore.Layout.ModelBinding;

namespace Plato.Files.Sharing.Controllers
{
    public class HomeController : Controller, IUpdateModel
    {

        private readonly IFileViewIncrementer<File> _fileViewIncrementer;
        private readonly IFileInviteStore<FileInvite> _fileInviteStore;
        private readonly IEmailFileInviteService _shareInviteService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IContextFacade _contextFacade;
        private readonly IFileStore<File> _fileStore;
        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public HomeController(            
            IStringLocalizer<AdminController> stringLocalizer,
            IHtmlLocalizer<AdminController> htmlLocalizer,
            IFileViewIncrementer<File> fileViewIncrementer,
            IFileInviteStore<FileInvite> fileInviteStore,
            IAuthorizationService authorizationService,
            IEmailFileInviteService shareInviteService,             
            IContextFacade contextFacade,
            IFileStore<File> fileStore,            
            IAlerter alerter)
        {

            _authorizationService = authorizationService;
            _fileViewIncrementer = fileViewIncrementer;
            _shareInviteService = shareInviteService;
            _fileInviteStore = fileInviteStore;            
            _contextFacade = contextFacade;
            _fileStore = fileStore;
            _alerter = alerter;

            T = htmlLocalizer;
            S = stringLocalizer;

        }

        // ---------------
        // Index
        // ---------------

        public async Task Index(int id, string token)
        {

            // Ensure we have a valid id
            if (id <= 0)
            {
                Response.StatusCode = StatusCodes.Status302Found;
                Response.Headers.Add(HeaderNames.Location, StatusCodePagePaths.NotFound);
                return;
            }

            // Ensure we have a token
            if (string.IsNullOrEmpty(token))
            {
                Response.StatusCode = StatusCodes.Status302Found;
                Response.Headers.Add(HeaderNames.Location, StatusCodePagePaths.NotFound);
                return;
            }

            // Ensure we have a valid base 64 token
            if (!token.IsBase64String())
            {
                Response.StatusCode = StatusCodes.Status302Found;
                Response.Headers.Add(HeaderNames.Location, StatusCodePagePaths.NotFound);
                return;
            }

            // Get invite
            var invite = await _fileInviteStore.GetByIdAsync(id);

            // Ensure invite exists
            if (invite == null)
            {
                Response.StatusCode = StatusCodes.Status302Found;
                Response.Headers.Add(HeaderNames.Location, StatusCodePagePaths.NotFound);
                return;
            }

            // Get file
            var file = await _fileStore.GetByIdAsync(invite.FileId);

            // Ensure file exists
            if (file == null)
            {
                Response.StatusCode = StatusCodes.Status302Found;
                Response.Headers.Add(HeaderNames.Location, StatusCodePagePaths.NotFound);
                return;
            }

            if (file.ContentLength <= 0)
            {
                Response.StatusCode = StatusCodes.Status302Found;
                Response.Headers.Add(HeaderNames.Location, StatusCodePagePaths.NotFound);
                return;
            }

            // Decode email
            var email = Encoding.UTF8.GetString(Convert.FromBase64String(token));

            // Ensure supplied decoded email matches the invite email
            if (!email.Equals(invite.Email, StringComparison.OrdinalIgnoreCase))
            {
                Response.StatusCode = StatusCodes.Status302Found;
                Response.Headers.Add(HeaderNames.Location, StatusCodePagePaths.Unauthorized);
            }

            // Increment file view count
            await _fileViewIncrementer
                .Contextulize(HttpContext)
                .IncrementAsync(file);

            // Serve file
            Response.Clear();
            Response.ContentType = file.ContentType;
            Response.Headers.Add(HeaderNames.ContentDisposition, "filename=\"" + file.Name + "\"");
            Response.Headers.Add(HeaderNames.ContentLength, Convert.ToString((long)file.ContentLength));
            await Response.Body.WriteAsync(file.ContentBlob, 0, (int)file.ContentLength);

        }

    }

}
