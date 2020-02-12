using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Models;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;
using PlatoCore.Layout.Views.Abstractions;

namespace Plato.Discuss.ViewComponents
{

    public class TopicViewComponent : ViewComponentBase
    {

        private readonly IEntityStore<Topic> _entityStore;

        public TopicViewComponent(IEntityStore<Topic> entityStore)
        {     
            _entityStore = entityStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(EntityViewModel<Topic, Reply> model)
        {

            if (model == null)
            {
                model = new EntityViewModel<Topic, Reply>();
            }

            return View(await GetViewModel(model));

        }

        async Task<EntityViewModel<Topic, Reply>> GetViewModel(EntityViewModel<Topic, Reply> model)
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
