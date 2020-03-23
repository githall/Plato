using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PlatoCore.Emails.Abstractions;
using PlatoCore.Layout.ModelBinding;
using Plato.Site.ViewModels;

namespace Plato.Site.Controllers
{
    public class PlatoController : Controller, IUpdateModel
    {

        #region "Constructor"

        public PlatoController()
        {        
        }

        #endregion

        #region "Actions"

        // ---------------------
        // /Index
        // ---------------------

        [HttpGet, AllowAnonymous]
        public Task<IActionResult> Index()
        {
            // Return view
            return Task.FromResult((IActionResult) View());

        }
    
        // ---------------------
        // Discuss
        // ---------------------

        [HttpGet, AllowAnonymous]
        public Task<IActionResult> Discuss()
        {
            // Return view
            return Task.FromResult((IActionResult)View());
        }

        // ---------------------
        // Docs
        // ---------------------

        [HttpGet, AllowAnonymous]
        public Task<IActionResult> Docs()
        {
            // Return view
            return Task.FromResult((IActionResult)View());
        }

        // ---------------------
        // Articles
        // ---------------------

        [HttpGet, AllowAnonymous]
        public Task<IActionResult> Articles()
        {
            // Return view
            return Task.FromResult((IActionResult)View());
        }

        // ---------------------
        // Questions
        // ---------------------

        [HttpGet, AllowAnonymous]
        public Task<IActionResult> Questions()
        {
            // Return view
            return Task.FromResult((IActionResult)View());
        }

        // ---------------------
        // Ideas
        // ---------------------

        [HttpGet, AllowAnonymous]
        public Task<IActionResult> Ideas()
        {
            // Return view
            return Task.FromResult((IActionResult)View());
        }

        // ---------------------
        // Issues
        // ---------------------

        [HttpGet, AllowAnonymous]
        public Task<IActionResult> Issues()
        {
            // Return view
            return Task.FromResult((IActionResult)View());
        }

        // ---------------------
        // Features
        // ---------------------

        [HttpGet, AllowAnonymous]
        public Task<IActionResult> Features()
        {
            // Return view
            return Task.FromResult((IActionResult) View());

        }

        // ---------------------
        // Modules
        // ---------------------

        [HttpGet, AllowAnonymous]
        public Task<IActionResult> Modules()
        {
            // Return view
            return Task.FromResult((IActionResult) View());

        }

        // ---------------------
        // Desktop
        // ---------------------

        [HttpGet, AllowAnonymous]
        public Task<IActionResult> Desktop()
        {
            // Return view
            return Task.FromResult((IActionResult)View());

        }

        #endregion

    }

}
