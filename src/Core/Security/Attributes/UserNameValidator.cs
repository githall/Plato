using System;
using System.Linq;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Text;
using PlatoCore.Abstractions;
using PlatoCore.Security.Abstractions;

namespace PlatoCore.Security.Attributes
{

    /// <summary>
    /// A custom view model validation attribute to ensure usernames obay configured user name options.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class UserNameValidator : ValidationAttribute
    {

        public UserNameValidator()
        {
            // TODO: Update error to reflect condifured UserNameOptions
            var options = new UserNameOptions();
            ErrorMessage = BuildError(options);
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {

            // Get password
            var userName = ((string)value);

            var userNameOptions = (IOptions<UserNameOptions>)context.GetService(typeof(IOptions<UserNameOptions>));
            var result = ValidateUserName(userName, userNameOptions.Value);

            // Return the first validation error we encounter
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    return new ValidationResult(error.Description);
                }
            }

            return ValidationResult.Success;
        }

        ICommandResultBase ValidateUserName(string username, UserNameOptions options)
        {

            var result = new CommandResultBase();
            var friendlyBlackList = BuildFriendlyBlackList(options.BlackListedCharacters.ToArray());

            if (string.IsNullOrWhiteSpace(username) || username.Length < options.RequiredLength)
            {
                result.Failed($"The username must be at least {options.RequiredLength} characters in length");
            }
            
            foreach (var character in options.BlackListedCharacters)
            {
                if (username.IndexOf(character) >= 0)
                {
                    result.Failed($"The username cannot contain {friendlyBlackList} characters");
                }
            }

            return result.Errors.Any()
                ? result.Failed()
                : result.Success();

        }

        string BuildError(UserNameOptions options)
        {
            var sb = new StringBuilder();
            var friendlyBlackList = BuildFriendlyBlackList(options.BlackListedCharacters.ToArray());
            sb.Append("The {0} field cannot contain ")
                .Append(friendlyBlackList);
            if (options.RequiredLength > 0)
            {
                sb.Append(" and must be at least ")
                    .Append(options.RequiredLength)
                    .Append(" characters in length");
            }
            return sb.ToString();
        }

        string BuildFriendlyBlackList(char[] blackList)
        {

            var i = 1;
            var sb = new StringBuilder();
            foreach (var character in blackList)
            {
                switch (character.ToString())
                {
                    case "  ":
                        sb.Append("tabs");
                        break;
                    case " ":
                        sb.Append("spaces");
                        break;
                    default:
                        sb.Append(character);
                        break;
                }

                if (i == blackList.Length - 1)
                {
                    sb.Append(" or ");                    
                }
                else
                {
                    sb.Append(" ");
                }

                i++;

            }

            return sb.ToString();

        }

    }

}
