﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Data.Abstractions;
using Plato.WebApi.Controllers;
using Plato.WebApi.Models;
using PlatoCore.Hosting.Web.Abstractions;

namespace Plato.Entities.Controllers
{

    public class EntityController : BaseWebApiController
    {

        private readonly IEntityStore<Entity> _entityStore;
        private readonly IContextFacade _contextFacade;

        public EntityController(
            IUrlHelperFactory urlHelperFactory,
            IContextFacade contextFacade,
            IEntityStore<Entity> entityStore)
        {
            _contextFacade = contextFacade;
            _entityStore = entityStore;
        }

        #region "Actions"

        [HttpGet]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> Get(
            int page = 1,
            int size = 10,
            string keywords = "",
            string sort = "LastReplyDate",
            OrderBy order = OrderBy.Desc)
        {

            var entities = await GetEntities(
                page,
                size,
                keywords,
                sort,
                order);

            IPagedResults<EntityApiResult> results = null;
            if (entities != null)
            {
                results = new PagedResults<EntityApiResult>
                {
                    Total = entities.Total
                };

                var baseUrl = await _contextFacade.GetBaseUrlAsync();
                foreach (var entity in entities.Data)
                {

                    var createdByUrl = baseUrl + _contextFacade.GetRouteUrl(new RouteValueDictionary()
                    {
                        ["area"] = "Plato.Users",
                        ["controller"] = "Home",
                        ["action"] = "Display",
                        ["opts.id"] = entity.CreatedBy.Id,
                        ["opts.alias"] = entity.CreatedBy.Alias
                    });

                    var modifiedByUrl = baseUrl + _contextFacade.GetRouteUrl(new RouteValueDictionary()
                    {
                        ["area"] = "Plato.Users",
                        ["controller"] = "Home",
                        ["action"] = "Display",
                        ["opts.id"] = entity.ModifiedBy.Id,
                        ["opts.alias"] = entity.ModifiedBy.Alias
                    });

                    var url = _contextFacade.GetRouteUrl(new RouteValueDictionary()
                    {
                        ["area"] = entity.ModuleId,
                        ["controller"] = "Home",
                        ["action"] = "Display",
                        ["opts.id"] = entity.Id,
                        ["opts.alias"] = entity.Alias
                    });

                    results.Data.Add(new EntityApiResult()
                    {
                        Id = entity.Id,
                        CreatedBy = new UserApiResult()
                        {
                            Id = entity.CreatedBy.Id,
                            DisplayName = entity.CreatedBy.DisplayName,
                            UserName = entity.CreatedBy.UserName,
                            Url = createdByUrl
                        },
                        ModifiedBy = new UserApiResult()
                        {
                            Id = entity.ModifiedBy.Id,
                            DisplayName = entity.ModifiedBy.DisplayName,
                            UserName = entity.ModifiedBy.UserName,
                            Url = modifiedByUrl
                        },
                        LastReplyBy = new UserApiResult()
                        {
                            Id = entity.ModifiedBy.Id,
                            DisplayName = entity.ModifiedBy.DisplayName,
                            UserName = entity.ModifiedBy.UserName,
                            Url = modifiedByUrl
                        },
                        Title = entity.Title,
                        Message = entity.Message,
                        Url = url,
                        CreatedDate = new FriendlyDate()
                        {
                            Text = entity.CreatedDate.ToPrettyDate(),
                            Value = entity.CreatedDate
                        }
                    });

                }

            }

            IPagedApiResults<EntityApiResult> output = null;
            if (results != null)
            {
                output = new PagedApiResults<EntityApiResult>()
                {
                    Page = page,
                    Size = size,
                    Total = results.Total,
                    TotalPages = results.Total.ToSafeCeilingDivision(size),
                    Data = results.Data
                };
            }

            return output != null
                ? base.Result(output)
                : base.NoResults();

        }
        
        #endregion

        #region "Private Methods"

        async Task<IPagedResults<Entity>> GetEntities(
            int page,
            int pageSize,
            string keywords,
            string sortBy,
            OrderBy sortOrder)
        {

            return await _entityStore.QueryAsync()
                .Take(page, pageSize)
                .Select<EntityQueryParams>(q =>
                {

                    if (!String.IsNullOrEmpty(keywords))
                    {
                        q.Keywords.Like(keywords);
                    }

                })
                .OrderBy(sortBy, sortOrder)
                .ToList();

        }

        #endregion

    }

}
