using Plato.Categories.ViewModels;
using Plato.Entities.ViewModels;
using PlatoCore.Navigation.Abstractions;

namespace Plato.Issues.Categories.ViewModels
{
    public class CategoryIndexViewModel 
    {
        
        public PagerOptions Pager { get; set; }

        public CategoryIndexOptions Options { get; set; }

        public EntityIndexOptions EntityIndexOptions { get; set; }
        
    }

}
