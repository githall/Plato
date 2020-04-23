using Microsoft.AspNetCore.Http;

namespace PlatoCore.Net.Abstractions
{
    public interface ICookieBuilder
    {
        ICookieBuilder Contextulize(HttpContext context);

        ICookieBuilder Append(string key, string value);

        ICookieBuilder Append(string key, string value, CookieOptions options);

        ICookieBuilder Delete(string key);

        ICookieBuilder Delete(string key, CookieOptions options);

        string Get(string key);

        string BuildKey(string key);

    }

}
