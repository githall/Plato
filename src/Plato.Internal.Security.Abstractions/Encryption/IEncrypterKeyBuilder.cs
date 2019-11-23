namespace Plato.Internal.Security.Abstractions.Encryption
{

    public interface IEncrypterKeyBuilder
    {

        string Key { get; }

        string Vector { get; }

    }

}
