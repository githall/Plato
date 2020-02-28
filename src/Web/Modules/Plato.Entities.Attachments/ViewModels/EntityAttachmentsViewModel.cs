using Plato.Attachments.Models;
using Plato.Entities.Attachments.Models;
using PlatoCore.Data.Abstractions;
using System.Collections.Generic;

namespace Plato.Entities.Attachments.ViewModels
{

    public class EntityAttachmentsViewModel
    {

        public IEnumerable<EntityAttachment> Results { get; set; }

    }
}
