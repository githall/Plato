using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Plato.Articles.Models;
using Plato.Articles.Share.ViewModels;
using PlatoCore.Hosting.Web.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;

namespace Plato.Articles.Share.ViewProviders
{
    public class ArticleViewProvider : ViewProviderBase<Article>
    {

        private readonly IContextFacade _contextFacade;

        public ArticleViewProvider(IContextFacade contextFacade)
        {
            _contextFacade = contextFacade;
        }

        public override Task<IViewProviderResult> BuildIndexAsync(Article entity, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override async Task<IViewProviderResult> BuildDisplayAsync(Article entity, IViewProviderContext context)
        {
            
            // Build view model
            var baseUrl = await _contextFacade.GetBaseUrlAsync();
            var viewModel = new ShareViewModel
            {
                Url = baseUrl + _contextFacade.GetRouteUrl(new RouteValueDictionary()
                {
                    ["area"] = "Plato.Articles",
                    ["controller"] = "Home",
                    ["action"] = "Display",
                    ["opts.id"] = entity.Id,
                    ["opts.alias"] = entity.Alias
                })
            };

            return Views(
                View<ShareViewModel>("Article.Share.Display.Sidebar", model => viewModel)
                    .Zone("content-right")
                    .Order(int.MaxValue - 100)
            );

        }
        
        public override Task<IViewProviderResult> BuildEditAsync(Article entity, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
               
        public override Task<IViewProviderResult> BuildUpdateAsync(Article entity, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
    }

}
