﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Plato.Discuss.Models;
using Plato.Discuss.Share.ViewModels;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;
using PlatoCore.Hosting.Web.Abstractions;
using PlatoCore.Layout.ModelBinding;

namespace Plato.Discuss.Share.Controllers
{
    public class HomeController : Controller, IUpdateModel
    {

        private readonly IEntityStore<Topic> _entityStore;
        private readonly IContextFacade _contextFacade;
      
        public HomeController(
            IEntityStore<Topic> entityStore,
            IContextFacade contextFacade)
        {
            _entityStore = entityStore;
            _contextFacade = contextFacade;
        }

        // Share dialog

        public async Task<IActionResult> Index(EntityOptions opts)
        {

            // We always need an entity Id
            if (opts.Id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(opts.Id));
            }

            // We always need an entity
            var entity = await _entityStore.GetByIdAsync(opts.Id);
            if (entity == null)
            {
                return NotFound();
            }
      
            // Build route values
            RouteValueDictionary routeValues = null;

            // Append offset
            if (opts.ReplyId > 0)
            {
                routeValues = new RouteValueDictionary()
                {
                    ["area"] = "Plato.Discuss",
                    ["controller"] = "Home",
                    ["action"] = "Reply",
                    ["opts.id"] = entity.Id,
                    ["opts.alias"] = entity.Alias,
                    ["opts.replyId"] = opts.ReplyId
                };
            }
            else
            {
                routeValues = new RouteValueDictionary()
                {
                    ["area"] = "Plato.Discuss",
                    ["controller"] = "Home",
                    ["action"] = "Display",
                    ["opts.id"] = entity.Id,
                    ["opts.alias"] = entity.Alias
                };
            }

            // Build view model
            var baseUrl = await _contextFacade.GetBaseUrlAsync();
            var viewModel = new ShareViewModel
            {
                Url = baseUrl + _contextFacade.GetRouteUrl(routeValues)
            };

            // Return view
            return View(viewModel);

        }
        
    }

}
