using Microsoft.AspNetCore.Routing;
using PlatoCore.Security.Abstractions;

namespace Plato.Entities.Attachments.ViewModels
{
    public class EntityAttachmentOptions
    {

        public string GuidHtmlName { get; set; }

        public string Guid { get; set; }

        public int EntityId { get; set; }

        public RouteValueDictionary PostRoute { get; set; }

        public RouteValueDictionary DeleteRoute { get; set; }

        public RouteValueDictionary EditRoute { get; set; }

        public RouteValueDictionary PreviewRoute { get; set; }

        public IPermission PostPermission { get; set; }

        public IPermission DeleteOwnPermission { get; set; }

        public IPermission DeleteAnyPermission { get; set; }

    }

}
