using PlatoCore.Data.Abstractions;
using Plato.Attachments.Models;

namespace Plato.Attachments.ViewModels
{
    public class AttachmentsViewModel
    {

        public IPagedResults<Attachment> Results { get; set; }

    }

}
