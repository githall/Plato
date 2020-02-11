using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlatoCore.Repositories.Abstract
{

    public interface IDictionaryRepository<T> : IRepository<T> where T : class
    {
        Task<IEnumerable<T>> SelectEntries();

        Task<T> SelectEntryByKey(string key);

        Task<bool> DeleteByKeyAsync(string key);

    }

}
