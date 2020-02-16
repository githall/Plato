using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Data.Tracing.ViewModels;
using PlatoCore.Data.Tracing.Abstractions;

namespace Plato.Data.Tracing.ViewComponents
{

    public class DbTraceListViewComponent : ViewComponent
    {

        private readonly IDbTraceState _dbTraceState;

        public DbTraceListViewComponent(IDbTraceState dbTraceState)
        {
            _dbTraceState = dbTraceState;
        }

        public Task<IViewComponentResult> InvokeAsync()
        {
            return Task.FromResult((IViewComponentResult) View(new TraceViewModel()
            {
                Traces = _dbTraceState.Traces
            }));
        }

    }

}
