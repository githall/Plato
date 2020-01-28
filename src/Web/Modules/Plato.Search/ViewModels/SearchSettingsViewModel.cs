using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using PlatoCore.Data.Abstractions;
using Plato.Search.Models;

namespace Plato.Search.ViewModels
{

    public class SearchSettingsViewModel
    {

        [Required]
        public SearchTypes SearchType { get; set; }

        public IEnumerable<SelectListItem> AvailableSearchTypes { get; set; }

        public IEnumerable<FullTextCatalog> Catalogs { get; set; }

        public IEnumerable<FullTextIndex> Indexes { get; set; }

    }

}
