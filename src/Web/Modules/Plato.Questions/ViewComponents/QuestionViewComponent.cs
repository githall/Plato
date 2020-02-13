using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Questions.Models;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;
using PlatoCore.Layout.Views.Abstractions;

namespace Plato.Questions.ViewComponents
{

    public class QuestionViewComponent : ViewComponentBase
    {

        private readonly IEntityStore<Question> _entityStore;

        public QuestionViewComponent(IEntityStore<Question> entityStore)
        {          
            _entityStore = entityStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(EntityViewModel<Question, Answer> model)
        {

            if (model == null)
            {
                model = new EntityViewModel<Question, Answer>();
            }

            if (model.Options == null)
            {
                model.Options = new EntityOptions();
            }

            return View(await GetViewModel(model));

        }

        async Task<EntityViewModel<Question, Answer>> GetViewModel(EntityViewModel<Question, Answer> model)
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
                    throw new ArgumentNullException(nameof(entity));
                }

                model.Entity = entity;

            }        

            return model;

        }

    }

}