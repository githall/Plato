using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PlatoCore.Data.Abstractions;
using PlatoCore.Models.Metrics;
using PlatoCore.Models.Users;
using PlatoCore.Repositories.Reputations;
using PlatoCore.Stores.Abstractions.Users;
using PlatoCore.Stores.Users;
using Plato.Reports.ViewModels;

namespace Plato.Reports.TopUsers.ViewComponents
{
    public class UserRepTopListViewComponent : ViewComponent
    {

        private readonly IAggregatedUserReputationRepository _aggregatedUserReputationRepository;
        private readonly IPlatoUserStore<User> _platoUserStore;

        public UserRepTopListViewComponent(
            IAggregatedUserReputationRepository aggregatedUserReputationRepository,
            IPlatoUserStore<User> platoUserStore)
        {
            _aggregatedUserReputationRepository = aggregatedUserReputationRepository;
            _platoUserStore = platoUserStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            ReportOptions options,
            ChartOptions chart)
        {
            
            if (options == null)
            {
                options = new ReportOptions();
            }

            if (chart == null)
            {
                chart = new ChartOptions();
            }
            
            return View(new ChartViewModel<IEnumerable<AggregatedModel<int, User>>>()
            {
                Options = chart,
                Data = await SelectUsersByReputationAsync(options)
            });

        }

        async Task<IEnumerable<AggregatedModel<int, User>>> SelectUsersByReputationAsync(ReportOptions options)
        {

            // Get reputation for specified range
            var viewsById = options.FeatureId > 0
                ? await _aggregatedUserReputationRepository.SelectSummedByIntAsync(
                    "CreatedUserId",
                    options.Start,
                    options.End,
                    options.FeatureId)
                : await _aggregatedUserReputationRepository.SelectSummedByIntAsync(
                    "CreatedUserId",
                    options.Start,
                    options.End);

            // Get all users matching awarded rep
            IPagedResults<User> users = null;
            if (viewsById != null)
            {
                users = await _platoUserStore.QueryAsync()
                    .Take(100, false)
                    .Select<UserQueryParams>(q =>
                    {
                        q.Id.IsIn(viewsById.Data.Select(d => d.Aggregate).ToArray());
                    })
                    .OrderBy("CreatedDate", OrderBy.Desc)
                    .ToList();
            }

            // Build total rep awarded and user
            List<AggregatedModel<int, User>> topUsers = null;
            if (users?.Data != null)
            {
                foreach (var entity in users.Data)
                {
                    // Get or add aggregate
                    var aggregate = viewsById?.Data.FirstOrDefault(m => m.Aggregate == entity.Id);
                    if (aggregate != null)
                    {
                        if (topUsers == null)
                        {
                            topUsers = new List<AggregatedModel<int, User>>();
                        }
                        topUsers.Add(new AggregatedModel<int, User>(aggregate, entity));
                    }
                }
            }

            return topUsers?.OrderByDescending(o => o.Aggregate.Count) ?? null;

        }
        
    }

}
