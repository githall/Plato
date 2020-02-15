using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using Plato.Entities.Models;

namespace Plato.Entities.ViewModels
{
    public class EntityTreeViewModel
    {
        
        public IList<Selection<ISimpleEntity>> SelectedEntities { get; set; }

        public IEnumerable<ISimpleEntity> SelectedParents { get; set; }

        public string HtmlName { get; set; }

        public bool EnableCheckBoxes { get; set; }

        public string EditMenuViewName { get; set; }

        public string CssClass { get; set; }

        public RouteValueDictionary RouteValues { get; set; }

    }

    public class Selection<TModel> where TModel : class, ISimpleEntity
    {

        public bool IsSelected { get; set; }

        public TModel Value { get; set; }

    }

}
