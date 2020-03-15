using PlatoCore.Abstractions;

namespace Plato.Labels.Services
{

    public interface ILabelManager<TLabel> : ICommandManager<TLabel> where TLabel : class
    {   
    }

}
