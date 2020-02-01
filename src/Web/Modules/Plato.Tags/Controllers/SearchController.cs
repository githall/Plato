using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Data.Abstractions;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Models.Features;
using PlatoCore.Stores.Abstractions.Shell;
using Plato.Tags.Models;
using Plato.Tags.Stores;
using Plato.WebApi.Controllers;
using Plato.WebApi.Models;

namespace Plato.Tags.Controllers
{

    public class SearchController : BaseWebApiController
    {

        private readonly IShellFeatureStore<ShellFeature> _shellFeatureStore;
        private readonly ITagStore<TagBase> _tagStore;
        private readonly IContextFacade _contextFacade;

        public SearchController(
            IShellFeatureStore<ShellFeature> shellFeatureStore,
            IContextFacade contextFacade,
            ITagStore<TagBase> tagStore)
        {
            _shellFeatureStore = shellFeatureStore;
            _contextFacade = contextFacade;
            _tagStore = tagStore;
        }        

        [HttpPost, ResponseCache(NoStore = true)]
        public async Task<IActionResult> Index([FromBody] TagApiParams parameters)
        {

            // We always need parameters
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            // Get tags
            var tags = await GetTags(parameters);

            // Build results
            IPagedResults<TagApiResult> results = null;
            if (tags != null)
            {

                // Get feature for tags
                IShellFeature feature = null;
                if (parameters.FeatureId > 0)
                {
                    feature = await _shellFeatureStore.GetByIdAsync(parameters.FeatureId);
                }
                
                results = new PagedResults<TagApiResult>
                {
                    Total = tags.Total
                };

                var baseUrl = await _contextFacade.GetBaseUrlAsync();
                foreach (var tag in tags.Data)
                {

                    var url = _contextFacade.GetRouteUrl(new RouteValueDictionary()
                    {
                        ["area"] = feature?.ModuleId ?? "Plato.Tags",
                        ["controller"] = "Home",
                        ["action"] = "Tag",
                        ["opts.id"] = tag.Id,
                        ["opts.alias"] = tag.Alias
                    });

                    results.Data.Add(new TagApiResult()
                    {
                        Id = tag.Id,
                        Name = tag.Name,
                        Entities = tag.TotalEntities.ToPrettyInt(),
                        Follows = tag.TotalFollows.ToPrettyInt(),
                        Url = url
                    });

                }

            }

            IPagedApiResults<TagApiResult> output = null;
            if (results != null)
            {
                output = new PagedApiResults<TagApiResult>()
                {
                    Page = parameters.Page,
                    Size = parameters.Size,
                    Total = results.Total,
                    TotalPages = results.Total.ToSafeCeilingDivision(parameters.Size),
                    Data = results.Data
                };
            }

            return output != null
                ? base.Result(output)
                : base.NoResults();

        }
           
        // ----------------

        async Task<IPagedResults<TagBase>> GetTags(TagApiParams parameters)
        {

            return await _tagStore.QueryAsync()
                .Take(parameters.Page, parameters.Size)
                .Select<TagQueryParams>(q =>
                {

                    if (parameters.FeatureId > 0)
                    {
                        q.FeatureId.Equals(parameters.FeatureId);
                    }

                    if (!String.IsNullOrEmpty(parameters.Keywords))
                    {
                        q.Keywords.Like(parameters.Keywords);
                    }

                })
                .OrderBy(parameters.Sort, parameters.Order)
                .ToList();

        }

    }

}
