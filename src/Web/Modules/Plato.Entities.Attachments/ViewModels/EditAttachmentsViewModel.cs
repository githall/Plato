using PlatoCore.Security.Abstractions;

namespace Plato.Entities.Attachments.ViewModels
{
    public class EditEntityAttachmentsViewModel
    {

        public string ContentGuid { get; set; }
       
        public IPermission Permission { get; set; }
        
    }

}
