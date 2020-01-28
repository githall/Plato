using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PlatoCore.Models.Users;
using Plato.Moderation.Models;

namespace Plato.Discuss.Categories.Moderators.ViewComponents
{
    public class ModeratorListItemViewComponent : ViewComponent
    {
   
        public Task<IViewComponentResult> InvokeAsync(
            Moderator moderator)
        {
            return Task.FromResult((IViewComponentResult) View(moderator));
        }
    

    }


}

