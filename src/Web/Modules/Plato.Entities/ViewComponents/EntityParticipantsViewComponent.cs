using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;
using PlatoCore.Data.Abstractions;
using PlatoCore.Layout.Views.Abstractions;

namespace Plato.Entities.ViewComponents
{

    public class EntityParticipantsViewComponent : ViewComponentBase
    {

        private readonly IEntityUsersStore _entityUsersStore;

        public EntityParticipantsViewComponent(
            IEntityUsersStore entityUsersStore)
        {
            _entityUsersStore = entityUsersStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(int id)
        {

            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }
            
            return View(await GetViewModel(id));

        }

        async Task<EntityParticipantsViewModel> GetViewModel(int id)
        {
            
            // Get top 10 participants
            var users = await _entityUsersStore.QueryAsync()
                .Take(10, false)
                .Select<EntityUserQueryParams>(q =>
                {
                    q.EntityId.Equals(id);
                })
                .OrderBy("t.TotalReplies", OrderBy.Desc)
                .ToList();

            return new EntityParticipantsViewModel()
            {
                Users = users
            };

        }

    }

}
