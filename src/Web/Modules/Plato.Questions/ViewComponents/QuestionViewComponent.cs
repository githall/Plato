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

        public async Task<IViewComponentResult> InvokeAsync(EntityOptions options)
        {

            if (options == null)
            {
                options = new EntityOptions();
            }

            var model = await GetViewModel(options);

            return View(model);

        }

        async Task<EntityViewModel<Question, Answer>> GetViewModel(EntityOptions options)
        {

            if (options.Id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(options.Id));
            }

            var entity = await _entityStore.GetByIdAsync(options.Id);
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            
            // Return view model
            return new EntityViewModel<Question, Answer>
            {
                Options = options,
                Entity = entity
            };

        }

    }

}