using Plato.Entities.Models;

namespace Plato.Docs.Flipper.ViewModels
{
    public class DocFlipperViewModel
    {

        public ISimpleEntity PreviousDoc { get; set; }

        public ISimpleEntity NextDoc { get; set; }

    }
}
