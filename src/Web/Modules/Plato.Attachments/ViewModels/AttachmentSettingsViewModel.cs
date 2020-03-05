using Plato.Roles.ViewModels;
using PlatoCore.Data.Abstractions;
using PlatoCore.Models.Roles;
using PlatoCore.Navigation.Abstractions;

namespace Plato.Attachments.ViewModels
{
    public class AttachmentSettingsViewModel : RolesIndexViewModel
    {

        public AttachmentSettingsViewModel()
        {
        }

        public AttachmentSettingsViewModel(
            IPagedResults<Role> results,
            RoleIndexOptions options,
            PagerOptions pager) : base(results, options, pager)
        {           
        }

    }

}
