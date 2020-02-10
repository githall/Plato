using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Articles.Models;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;
using PlatoCore.Layout.Views.Abstractions;

namespace Plato.Articles.ViewComponents
{

    public class ArticleViewComponent : ViewComponentBase
    {

        private readonly IEntityStore<Article> _entityStore;

        public ArticleViewComponent(IEntityStore<Article> entityStore)
        {
            _entityStore = entityStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(EntityOptions options)
        {

            if (options == null)
            {
                options = new EntityOptions();
            }

            var model = await GetViewModel(options);

            return View(model);

        }

        async Task<EntityViewModel<Article, Comment>> GetViewModel(
            EntityOptions options)
        {

            if (options.Id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(options.Id));
            }

            var entity = await _entityStore.GetByIdAsync(options.Id);
            if (entity == null)
            {
                throw new ArgumentNullException();
            }
            
            // Return view model
            return new EntityViewModel<Article, Comment>
            {
                Options = options,
                Entity = entity
        };

        }

    }

}
