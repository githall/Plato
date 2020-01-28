using Plato.Docs.Models;
using PlatoCore.Data.Abstractions;
using Plato.Entities.ViewModels;
using PlatoCore.Navigation.Abstractions;

namespace Plato.Docs.Tags.ViewModels
{
    public class TagDisplayViewModel
    {

        public PagerOptions Pager { get; set; }

        public EntityIndexOptions Options { get; set; }

    }

}
