using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PlatoCore.Layout.ModelBinding;
using Plato.Entities.Attachments.ViewModels;

namespace Plato.Articles.Attachments.Controllers
{
    public class HomeController : Controller, IUpdateModel
    {

        public HomeController()
        { 
        }

        // -----------
        // Edit
        // -----------

        public Task<IActionResult> Edit(EntityAttachmentOptions opts)
        {

            if (opts == null)
            {
                opts = new EntityAttachmentOptions();
            }

            // We always need a guid
            if (string.IsNullOrEmpty(opts.Guid))
            {
                throw new ArgumentNullException(nameof(opts.Guid));
            }

            opts.PostPermission = Permissions.PostArticleAttachments;
            opts.DeleteOwnPermission = Permissions.DeleteOwnArticleAttachments;
            opts.DeleteAnyPermission = Permissions.DeleteAnyArticleAttachments;

            // Return view
            return Task.FromResult((IActionResult)View(opts));


        }

        // -----------
        // Preview
        // -----------

        public Task<IActionResult> Preview(EntityAttachmentOptions opts)
        {

            if (opts == null)
            {
                opts = new EntityAttachmentOptions();
            }

            // We always need a guid
            if (string.IsNullOrEmpty(opts.Guid))
            {
                throw new ArgumentNullException(nameof(opts.Guid));
            }

            opts.PostPermission = Permissions.PostArticleAttachments;
            opts.DeleteOwnPermission = Permissions.DeleteOwnArticleAttachments;
            opts.DeleteAnyPermission = Permissions.DeleteAnyArticleAttachments;

            // Return view
            return Task.FromResult((IActionResult) View(opts));

        }

    }

}
