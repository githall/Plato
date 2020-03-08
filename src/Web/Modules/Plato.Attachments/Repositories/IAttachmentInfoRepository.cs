using System.Threading.Tasks;

namespace Plato.Attachments.Repositories
{

    public interface IAttachmentInfoRepository<TModel> where TModel : class
    {

        Task<TModel> SelectByUserIdAsync(int userId);

    }

}
