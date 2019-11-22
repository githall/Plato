using System.Threading.Tasks;

namespace Plato.Internal.Security.Abstractions.Encryption
{

    public interface IPlatoKeyStore
    {
        Task<PlatoKeys> GetOrCreateKeysAsync();        
    }

}
