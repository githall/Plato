using System.Threading.Tasks;
using PlatoCore.Abstractions.Settings;

namespace Plato.Settings.Services
{

    public interface ISiteSettingsManager
    {
        Task<ISiteSettings> SaveAsync(ISiteSettings siteSettings);
    }

}
