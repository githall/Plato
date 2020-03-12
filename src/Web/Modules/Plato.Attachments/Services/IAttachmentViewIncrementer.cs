using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Plato.Attachments.Services
{

    public interface IAttachmentViewIncrementer<TAttachment> where TAttachment : class
    {

        IAttachmentViewIncrementer<TAttachment> Contextulize(HttpContext context);

        Task<TAttachment> IncrementAsync(TAttachment attachment);

    }
}
