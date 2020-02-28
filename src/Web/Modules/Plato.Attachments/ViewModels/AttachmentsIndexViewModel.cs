using PlatoCore.Data.Abstractions;
using Plato.Attachments.Models;

namespace Plato.Attachments.ViewModels
{
    public class AttachmentsIndexViewModel
    {

        public IPagedResults<Attachment> Results { get; set; }

    }

}
