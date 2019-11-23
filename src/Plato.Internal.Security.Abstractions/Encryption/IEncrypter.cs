namespace Plato.Internal.Security.Abstractions.Encryption
{
    public interface IEncrypter
    {

        string Encrypt(string plainText);

        string Decrypt(string cipherText);

    }

}
