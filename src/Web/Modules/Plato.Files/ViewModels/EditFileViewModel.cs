using Microsoft.AspNetCore.Routing;
using Plato.Files.Models;

namespace Plato.Files.ViewModels
{
    public class EditFileViewModel
    {

        public Models.File File { get; set; }

        public FileInfo Info { get; set; }

        public FileOptions Options { get; set; }

        public string Name { get; set; }

        public RouteValueDictionary PostRoute { get; set; }

        public RouteValueDictionary ReturnRoute { get; set; }


    }

}
