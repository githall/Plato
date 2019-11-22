namespace Plato.Internal.Security.Abstractions.Encryption
{

    public interface IKeyInfo
    {

        byte[] Key { get; }

        byte[] Iv { get; }

    }

}
