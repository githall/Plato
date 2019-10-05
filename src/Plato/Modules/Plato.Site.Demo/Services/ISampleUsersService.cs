namespace Plato.Site.Demo.Services
{

    public interface ISampleUsersService : ISampleDataService
    {
        string[] Usernames { get; }
    }

}
