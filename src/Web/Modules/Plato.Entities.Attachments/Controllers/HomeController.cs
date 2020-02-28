using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Attachments.Stores;
using Plato.Entities.Attachments.ViewModels;
using Plato.Attachments.Models;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ModelBinding;

namespace Plato.Entities.Attachments.Controllers
{
    public class HomeController : Controller, IUpdateModel
    {

        private readonly IAttachmentStore<Attachment> _attachmentStore;

        //private readonly IEntityStore<Doc> _entityStore;
        private readonly IContextFacade _contextFacade;
      
        public HomeController(
            //IEntityStore<Doc> entityStore,
            IAttachmentStore<Attachment> attachmentStore,
            IContextFacade contextFacade)
        {
            //_entityStore = entityStore;
            _attachmentStore = attachmentStore;
            _contextFacade = contextFacade;
        }

        // Share dialog

        public async Task<IActionResult> Index(EntityAttachmentOptions opts)
        {


            //// We always need an entity Id
            //if (opts.Id <= 0)
            //{
            //    throw new ArgumentOutOfRangeException(nameof(opts.Id));
            //}

            //// We always need an entity
            //var entity = await _entityStore.GetByIdAsync(opts.Id);
            //if (entity == null)
            //{
            //    return NotFound();
            //}
      
            //// Build route values
            //RouteValueDictionary routeValues = null;

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


            var results = await _attachmentStore
                .QueryAsync()
                .Select<AttachmentQueryParams>(q => {
                    q.Keywords.Equals("");
                })
                .ToList();

            //// Build view model
            //var baseUrl = await _contextFacade.GetBaseUrlAsync();
            //var viewModel = new ShareViewModel
            //{
            //    Url = baseUrl + _contextFacade.GetRouteUrl(routeValues)
            //};

            var viewModel = new EntityAttachmentsIndexViewModel()
            {
                Results = results
            };

            // Return view
            return View(viewModel);

        }
        
    }

}
