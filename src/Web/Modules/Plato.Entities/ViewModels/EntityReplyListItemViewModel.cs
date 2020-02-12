using Plato.Entities.Models;
using PlatoCore.Layout.TagHelperAdapters.Abstractions;

namespace Plato.Entities.ViewModels
{
    public class EntityReplyListItemViewModel<TEntity, TReply>
        : TagHelperAdapterAwareViewModel
        where TEntity : class, IEntity
        where TReply : class, IEntityReply
    {

        public TReply Reply { get; set; }

        public TEntity Entity { get; set; }
        
    }

}
