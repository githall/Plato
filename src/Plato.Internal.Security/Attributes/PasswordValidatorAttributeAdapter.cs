using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Plato.Internal.Security.Attributes
{

    public class PasswordValidatorAttributeAdapter : AttributeAdapterBase<PasswordValidator>
    {                

        public PasswordValidatorAttributeAdapter(
            PasswordValidator attribute,
            IStringLocalizer stringLocalizer)
            : base(attribute, stringLocalizer)
        {
        
        }

        public override void AddValidation(ClientModelValidationContext context)
        {

            var httpContext = context.ActionContext.HttpContext;
            var identityOptions = httpContext.RequestServices.GetService<IOptions<IdentityOptions>>();

            if (identityOptions == null)
            {
                return;
            }

            var options = identityOptions.Value.Password;

            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-password", GetErrorMessage(context));

            if (options.RequireLowercase)
            {
                MergeAttribute(context.Attributes, "data-val-password-lower", "true");
            }

            if (options.RequireUppercase)
            {
                MergeAttribute(context.Attributes, "data-val-password-upper", "true");
            }

            if (options.RequireDigit)
            {
                MergeAttribute(context.Attributes, "data-val-password-digit", "true");
            }

            if (options.RequireNonAlphanumeric)
            {
                MergeAttribute(context.Attributes, "data-val-password-non-alpha-numeric", "true");
            }                       

            if (options.RequiredLength > 0)
            {
                MergeAttribute(context.Attributes, "data-val-password-length", options.RequiredLength.ToString());
            }

        }

        public override string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            return GetErrorMessage(validationContext.ModelMetadata,
                validationContext.ModelMetadata.GetDisplayName());
        }

    }


}
