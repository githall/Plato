using System.Threading.Tasks;

namespace PlatoCore.Security.Abstractions.Encryption
{

    public interface IEncrypterKeyStore
    {
        Task<EncrypterKeys> GetOrCreateKeysAsync();        
    }

}
