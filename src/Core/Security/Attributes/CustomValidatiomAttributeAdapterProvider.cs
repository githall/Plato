using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace PlatoCore.Security.Attributes
{

    public class CustomValidatiomAttributeAdapterProvider : IValidationAttributeAdapterProvider
    {

        IValidationAttributeAdapterProvider baseProvider = new ValidationAttributeAdapterProvider();

        public IAttributeAdapter GetAttributeAdapter(ValidationAttribute attribute,
            IStringLocalizer stringLocalizer)
        {

            if (attribute is UserNameValidator)
            {
                return new UserNameValidatorAttributeAdapter(attribute as UserNameValidator, stringLocalizer);
            }

            if (attribute is PasswordValidator)
            {
                return new PasswordValidatorAttributeAdapter(attribute as PasswordValidator, stringLocalizer);
            }
            
            return baseProvider.GetAttributeAdapter(attribute, stringLocalizer);

        }
    }

}
