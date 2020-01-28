using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using PlatoCore.Security.Abstractions;

namespace PlatoCore.Security.Attributes
{

    public class UserNameValidatorAttributeAdapter : AttributeAdapterBase<UserNameValidator>
    {

        UserNameValidator _attribute;

        public UserNameValidatorAttributeAdapter(
            UserNameValidator attribute,
            IStringLocalizer stringLocalizer)
            : base(attribute, stringLocalizer)
        {
            _attribute = attribute;
        }

        public override void AddValidation(ClientModelValidationContext context)
        {

            var httpContext = context.ActionContext.HttpContext;
            var userNameOptions = httpContext.RequestServices.GetService<IOptions<UserNameOptions>>();

            if (userNameOptions == null)
            {
                return;
            }

            var options = userNameOptions.Value;

            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-username", GetErrorMessage(context));
            MergeAttribute(context.Attributes, "data-val-username-length", options.RequiredLength.ToString());
            MergeAttribute(context.Attributes, "data-val-username-blacklist", options.BlackListedCharacters);
        }

        public override string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            return GetErrorMessage(validationContext.ModelMetadata, validationContext.ModelMetadata.GetDisplayName());
        }

    }

}
