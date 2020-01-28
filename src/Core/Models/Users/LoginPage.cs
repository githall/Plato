namespace PlatoCore.Models.Users
{
    public class LoginPage : User
    {
        // LoginPage is simply a marker class so we can use
        // a separate view provider for the front-end user login page
        // This class should not contain any code

        public bool RememberMe { get; set; }

    }

}
