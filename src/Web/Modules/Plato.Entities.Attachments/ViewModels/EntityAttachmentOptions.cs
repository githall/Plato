using PlatoCore.Security.Abstractions;

namespace Plato.Entities.Attachments.ViewModels
{
    public class EntityAttachmentOptions
    {

        public string GuidHtmlName { get; set; }

        public string Guid { get; set; }

        public int FeatureId { get; set; }

        public int EntityId { get; set; }

        public IPermission Permission { get; set; }

    }

}
