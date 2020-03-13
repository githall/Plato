using Microsoft.AspNetCore.Routing;

namespace Plato.Files.ViewModels
{
    public class EditFileViewModel
    {

        public Models.File File { get; set; }

        public string Name { get; set; }

        public bool IsNewFile { get; set; }

        public RouteValueDictionary ShareRoute { get; set; }

        public RouteValueDictionary PostRoute { get; set; }


    }

}
