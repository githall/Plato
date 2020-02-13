using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Issues.Models;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;
using PlatoCore.Layout.Views.Abstractions;

namespace Plato.Issues.ViewComponents
{

    public class IssueViewComponent : ViewComponentBase
    {

        private readonly IEntityStore<Issue> _entityStore;

        public IssueViewComponent(IEntityStore<Issue> entityStore)
        {           
            _entityStore = entityStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(EntityViewModel<Issue, Comment> model)
        {

            if (model == null)
            {
                model = new EntityViewModel<Issue, Comment>();
            }

            if (model.Options == null)
            {
                model.Options = new EntityOptions();
            }

            return View(await GetViewModel(model));

        }

        async Task<EntityViewModel<Issue, Comment>> GetViewModel(EntityViewModel<Issue, Comment> model)
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
  