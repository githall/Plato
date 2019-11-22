namespace Plato.Internal.Security.Abstractions.Encryption
{
    public interface IEncrypter
    {

        string Encrypt(string input);

        string Decrypt(string input);

    }

}
