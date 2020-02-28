using Plato.Attachments.Models;
using PlatoCore.Data.Abstractions;

namespace Plato.Entities.Attachments.ViewModels
{
    public class EntityAttachmentsIndexViewModel
    {

        public IPagedResults<Attachment> Results { get; set; }

    }
}
