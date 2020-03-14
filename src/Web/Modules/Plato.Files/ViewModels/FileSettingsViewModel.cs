using Plato.Roles.ViewModels;
using PlatoCore.Data.Abstractions;
using PlatoCore.Models.Roles;
using PlatoCore.Navigation.Abstractions;

namespace Plato.Files.ViewModels
{
    public class FileSettingsViewModel : RolesIndexViewModel
    {

        public FileSettingsViewModel()
        {
        }

        public FileSettingsViewModel(
            IPagedResults<Role> results,
            RoleIndexOptions options,
            PagerOptions pager) : base(results, options, pager)
        {           
        }

    }

}
