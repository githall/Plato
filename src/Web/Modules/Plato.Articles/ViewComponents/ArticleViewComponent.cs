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

        public async Task<IViewComponentResult> InvokeAsync(EntityViewModel<Article, Comment> model)
        {

            if (model == null)
            {
                model = new EntityViewModel<Article, Comment>();
            }

            if (model.Options == null)
            {
                model.Options = new EntityOptions();
            }
         
            return View(await GetViewModel(model));

        }

        async Task<EntityViewModel<Article, Comment>> GetViewModel(EntityViewModel<Article, Comment> model)
        {

            if (model.Entity == null)
            {

                if (model.Options.Id <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(model.Options.Id));
                }

                var entity = await _entityStore.GetByIdAsync(model.Options.Id);
                if (entity == null)
                {
                    throw new ArgumentNullException();
                }

                model.Entity = entity;

            }          

            return model;

        }

    }

}
