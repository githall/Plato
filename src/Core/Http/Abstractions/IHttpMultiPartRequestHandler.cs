using Microsoft.AspNetCore.Http;
using PlatoCore.Abstractions;
using System.Threading.Tasks;

namespace PlatoCore.Http.Abstractions
{
    public interface IHttpMultiPartRequestHandler
    {
        Task<ICommandResult<MultiPartRequestResult>> ProcessAsync(HttpRequest request);

    }

}
