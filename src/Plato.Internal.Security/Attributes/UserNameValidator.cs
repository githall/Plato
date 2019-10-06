using Microsoft.Extensions.Options;
using Plato.Internal.Abstractions;
using Plato.Internal.Security.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Plato.Internal.Security.Attributes
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
            ErrorMessage = "The {0} field cannot contain spaces or @ . , characters";
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {

            // Get password
            var userName = ((string)value);

            var identityOptions = (IOptions<UserNameOptions>)context.GetService(typeof(IOptions<UserNameOptions>));
            var result = ValidateUserName(userName, identityOptions.Value);

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

            if (string.IsNullOrWhiteSpace(username) || username.Length < options.RequiredLength)
            {
                result.Failed($"The username must be at least {options.RequiredLength} characters in length");
            }

            foreach (var character in options.BlackListedCharacters) {
                if (username.IndexOf(character) >= 0)
                {
                    result.Failed($"The username cannot contain the {character} character");
                }
            }

            return result.Errors.Any()
                ? result.Failed()
                : result.Success();

        }

    }

}
