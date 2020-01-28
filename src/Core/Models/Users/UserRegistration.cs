namespace PlatoCore.Models.Users
{
    public class UserRegistration : User
    {
        // UserRegistration  is simply a marker class so we can use
        // a separate view provider for the front-end user registration page        

        public bool IsNewUser { get; set; }

        public string ConfirmPassword { get; set; }

    }

}
