using System.Collections.Generic;
using System.Threading.Tasks;
using PlatoCore.Localization.Abstractions.Models;
using LocalizedString = Microsoft.Extensions.Localization.LocalizedString;

namespace PlatoCore.Localization.Abstractions
{

    public interface ILocaleStore
    {
  
        Task<IEnumerable<ComposedLocaleResource>> GetResourcesAsync(string cultureCode);

        Task<IEnumerable<LocalizedValues<TModel>>> GetResourcesAsync<TModel>(string cultureCode) where TModel : class, ILocalizedValue;

        Task<IEnumerable<LocalizedString>> GetAllStringsAsync(string cultureCode);

        Task DisposeAsync();
        
    }

}
