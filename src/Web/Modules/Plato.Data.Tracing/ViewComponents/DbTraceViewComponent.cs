using Microsoft.AspNetCore.Mvc;
using Plato.Data.Tracing.ViewModels;
using PlatoCore.Data.Tracing.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plato.Data.Tracing.ViewComponents
{

    public class DbTraceViewComponent : ViewComponent
    {

        private readonly IDbTraceState _dbTraceState;

        public DbTraceViewComponent(IDbTraceState dbTraceState)
        {
            _dbTraceState = dbTraceState;
        }

        public Task<IViewComponentResult> InvokeAsync(IEnumerable<IDbTrace> traces)
        {
            return Task.FromResult((IViewComponentResult) View(new TraceViewModel()
            {
                Traces = traces
            }));
        }

    }

}
