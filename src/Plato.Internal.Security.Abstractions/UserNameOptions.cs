namespace Plato.Internal.Security.Abstractions
{
    public class UserNameOptions
    {

        public int RequiredLength { get; set; } = 4;

        public string BlackListedCharacters { get; set; } = "@,. ";

    }

}
