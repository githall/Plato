using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Docs.Models;
using Plato.Entities.Models;
using PlatoCore.Layout.Views.Abstractions;

namespace Plato.Docs.ViewComponents
{

    public class DocTreeMenuViewComponent : ViewComponentBase
    {

        public DocTreeMenuViewComponent()
        {
        }

        public Task<IViewComponentResult> InvokeAsync(ISimpleEntity entity)
        {

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return Task.FromResult((IViewComponentResult) View((SimpleDoc) entity));

        }

    }

}
