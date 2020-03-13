using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Plato.Files.Models;
using Plato.Files.Stores;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Data.Abstractions;
using PlatoCore.Models.Shell;

namespace Plato.Files.Services
{

    public class AttachmentViewIncrementer : IAttachmentViewIncrementer<File>
    {

        public const string CookieName = "plato_attachment_views";
        private HttpContext _context;

        private readonly IFileStore<File> _attachmentStore;
        private readonly IShellSettings _shellSettings;        
        private readonly IDbHelper _dbHelper;

        public AttachmentViewIncrementer(
            IFileStore<File> attachmentStore,
            IShellSettings shellSettings,
            IDbHelper dbHelper)
        {
            _attachmentStore = attachmentStore;
            _shellSettings = shellSettings;
            _dbHelper = dbHelper;
        }

        public IAttachmentViewIncrementer<File> Contextulize(HttpContext context)
        {
            _context = context;
            return this;
        }

        public async Task<File> IncrementAsync(File attachment)
        {

            // Transform tracking cookie into int array
            List<int> values = null;

            var storage = "";
            if (_context != null)
            {
                storage = _context.Request.Cookies[CookieName];
            }

            // Read existing into values
            if (!String.IsNullOrEmpty(storage))
            {
                values = storage.ToIntArray().ToList();
            }

            // Does the entity Id we are accessing exist in our store
            if (values != null)
            {
                if (values.Contains(attachment.Id))
                {
                    return attachment;
                }
            }

            await UpdateTotalViewsAsync(attachment);


            if (values == null)
            {
                values = new List<int>();
            }

            values.Add(attachment.Id);

            var tennantPath = "/";
            if (_shellSettings != null)
            {
                tennantPath += _shellSettings.RequestedUrlPrefix;
            }

            // If a context is supplied use a client side cookie to track views
            // Expire the cookie every 10 minutes using a sliding expiration to
            // ensure views are updated often but not on every refresh
            _context?.Response.Cookies.Append(
                CookieName,
                values.ToArray().ToDelimitedString(),
                new CookieOptions
                {
                    HttpOnly = true,
                    Path = tennantPath,
                    Expires = DateTime.Now.AddMinutes(10)
                });

            return attachment;

        }

        async Task UpdateTotalViewsAsync(File attachment)
        {
            // Sql query
            const string sql = "UPDATE {prefix}_Attachments SET TotalViews = {views} WHERE Id = {id};";

            // Execute and return results
            await _dbHelper.ExecuteScalarAsync<int>(sql, new Dictionary<string, string>()
            {
                ["{id}"] = attachment.Id.ToString(),
                ["{views}"] = (attachment.TotalViews += 1).ToString()
            });

            _attachmentStore.CancelTokens(attachment);
        }

    }

}
