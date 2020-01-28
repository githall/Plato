namespace PlatoCore.Security.Abstractions.Encryption
{

    public interface IEncrypterKeyBuilder
    {

        string Key { get; }

        string Vector { get; }

    }

}
