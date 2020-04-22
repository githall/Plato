using System;
using System.Text;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace PlatoCore.Hosting.Web.Configuration
{

    public class AntiForgeryOptionsConfiguration : IConfigureOptions<AntiforgeryOptions>
    {

        private readonly DataProtectionOptions _dataProtectionOptions;

        public AntiForgeryOptionsConfiguration(IOptions<DataProtectionOptions> dataProtectionOptions)
        {
            _dataProtectionOptions = dataProtectionOptions.Value;
        }

        public const string DefaultCookiePrefix = "plato_csrf";

        public void Configure(AntiforgeryOptions options)
        {
     
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            options.FormFieldName = "plato-csrf";
            options.HeaderName = "X-Csrf-Token";

            if (options.Cookie.Name == null)
            {
                var applicationId = _dataProtectionOptions.ApplicationDiscriminator ?? string.Empty;
                options.Cookie.Name = $"{DefaultCookiePrefix}_{ComputeCookieName(applicationId)}";
            }

        }

        private static string ComputeCookieName(string applicationId)
        {
            using (var sha256 = CryptographyAlgorithms.CreateSHA256())
            {
                var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(applicationId));
                var subHash = hash.Take(8).ToArray();
                return WebEncoders.Base64UrlEncode(subHash);
            }
        }

    }

    internal static class CryptographyAlgorithms
    {
        public static SHA256 CreateSHA256()
        {
            try
            {
                return SHA256.Create();
            }
            // SHA256.Create is documented to throw this exception on FIPS compliant machines.
            // See: https://msdn.microsoft.com/en-us/library/z08hz7ad%28v=vs.110%29.aspx?f=255&MSPPError=-2147217396
            catch (System.Reflection.TargetInvocationException)
            {
                // Fallback to a FIPS compliant SHA256 algorithm.
                return new SHA256CryptoServiceProvider();
            }
        }
    }

}
