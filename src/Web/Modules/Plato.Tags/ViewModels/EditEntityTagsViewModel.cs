using System.ComponentModel.DataAnnotations;
using PlatoCore.Security.Abstractions;

namespace Plato.Tags.ViewModels
{

    public class EditEntityTagsViewModel
    {

        [Required, Display(Name = "tags")]
        public string Tags { get; set; }

        public string HtmlName { get; set; }

        public Permission Permission { get; set; }

        public int FeatureId { get; set; }

    }

}
