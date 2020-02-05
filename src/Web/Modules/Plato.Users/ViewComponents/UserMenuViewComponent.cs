using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Plato.Users.ViewComponents
{

    public class UserMenuViewComponent : ViewComponent
    {

        public UserMenuViewComponent()
        {     
        }

        public Task<IViewComponentResult> InvokeAsync()
        {            
            return Task.FromResult((IViewComponentResult) View());
        }

    }

}
