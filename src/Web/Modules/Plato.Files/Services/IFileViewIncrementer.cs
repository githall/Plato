using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Plato.Files.Services
{

    public interface IFileViewIncrementer<TFile> where TFile : class
    {

        IFileViewIncrementer<TFile> Contextulize(HttpContext context);

        Task<TFile> IncrementAsync(TFile file);

    }
}
