using PlatoCore.Data.Abstractions;
using Plato.Attachments.Models;
using PlatoCore.Security.Abstractions;

namespace Plato.Attachments.ViewModels
{
    public class AttachmentsViewModel
    {

        public IPagedResults<Attachment> Results { get; set; }

        public AttachmentInfo Info { get; set; }

        public AttachmentOptions Options { get; set; }

        public IPermission PostPermission { get; set; }

        public IPermission DeleteOwnPermission { get; set; }

        public IPermission DeleteAnyPermission { get; set; }

    }

}
