using Microsoft.AspNetCore.Routing;

namespace Plato.Site.Demo.Models
{

    public class EntityDataDescriptor
    {

        public string ModuleId { get; set; }

        public string EntityType { get; set; }
        
        public int EntitiesToCreate { get; set; } = 10;

        public RouteValueDictionary EntityRoute { get; set; }

        public RouteValueDictionary ReplyRoute { get; set; }

        public int EntityRepliesToCreate { get; set; } = 25;

    }

}

