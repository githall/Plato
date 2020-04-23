using System;
using Microsoft.AspNetCore.Http;
using PlatoCore.Models.Shell;
using PlatoCore.Net.Abstractions;

namespace PlatoCore.Net
{

    /// <summary>
    /// Provides per tenant cookie isolation.
    /// </summary>
    public class CookieBuilder : ICookieBuilder
    {

        private HttpContext _context;

        private readonly string _cookieSuffix = null;
        private readonly string _cookiePath = null;

        public CookieBuilder(IShellSettings shellSettings)
        {
            _cookieSuffix = shellSettings?.AuthCookieName?.ToLower() ?? string.Empty;
            _cookiePath = !string.IsNullOrEmpty(shellSettings?.RequestedUrlPrefix)
                ? $"/{shellSettings.RequestedUrlPrefix}"
                : "/";       
        }

        public ICookieBuilder Contextulize(HttpContext context)
        {

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            _context = context;
            return this;

        }

        public ICookieBuilder Append(string key, string value)
        {

            ValidateContext();

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(_context));
            }

            _context?.Response.Cookies.Append(
                BuildKeyInternal(key),
                value,
                new CookieOptions
                {
                    HttpOnly = true,
                    Path = _cookiePath
                });

            return this;

        }

        public ICookieBuilder Append(string key, string value, CookieOptions options)
        {

            ValidateContext();

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(_context));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            options.Path = _cookiePath;          

            _context?.Response.Cookies.Append(
                BuildKeyInternal(key),
                value,
                options);

            return this;

        }

        public ICookieBuilder Delete(string key)
        {
            ValidateContext();

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(_context));
            }

            _context?.Response.Cookies.Delete(BuildKeyInternal(key), new CookieOptions
            {
                HttpOnly = true,
                Path = _cookiePath
            });

            return this;

        }

        public ICookieBuilder Delete(string key, CookieOptions options)
        {
            ValidateContext();

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(_context));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            options.Path = _cookiePath;           

            _context?.Response.Cookies.Delete(BuildKeyInternal(key), options);

            return this;

        }

        public string Get(string key)
        {

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            var internalKey = BuildKeyInternal(key);
            if (_context.Request.Cookies.ContainsKey(internalKey))
            {
                return _context.Request.Cookies[internalKey];
            }

            return string.Empty;

        }

        public string BuildKey(string key)
        {
            return BuildKeyInternal(key);
        }

        // -----------------------

        private void ValidateContext()
        {
            if (_context == null)
            {
                throw new ArgumentNullException(nameof(_context));
            }
        }

        private string BuildKeyInternal(string key)
        {

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (key.EndsWith("_"))
            {
                return $"{key}{_cookieSuffix}";
            }

            return $"{key}_{_cookieSuffix}";

        }

    }

}
