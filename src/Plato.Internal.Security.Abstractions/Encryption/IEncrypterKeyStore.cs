using System.Threading.Tasks;

namespace Plato.Internal.Security.Abstractions.Encryption
{

    public interface IEncrypterKeyStore
    {
        Task<EncrypterKeys> GetOrCreateKeysAsync();        
    }

}
