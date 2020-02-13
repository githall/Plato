using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Ideas.Models;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;
using PlatoCore.Layout.Views.Abstractions;

namespace Plato.Ideas.ViewComponents
{

    public class IdeaViewComponent : ViewComponentBase
    {

        private readonly IEntityStore<Idea> _entityStore;

        public IdeaViewComponent(IEntityStore<Idea> entityStore)
        {
            _entityStore = entityStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(EntityViewModel<Idea, IdeaComment> model)
        {

            if (model == null)
            {
                model = new EntityViewModel<Idea, IdeaComment>();
            }

            if (model.Options == null)
            {
                model.Options = new EntityOptions();
            }

            return View(await GetViewModel(model));

        }

        async Task<EntityViewModel<Idea, IdeaComment>> GetViewModel(EntityViewModel<Idea, IdeaComment> model)
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

            return model;

        }

    }

}