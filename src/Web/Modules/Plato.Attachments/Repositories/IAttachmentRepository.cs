using PlatoCore.Repositories;

namespace Plato.Attachments.Repositories
{

    public interface IAttachmentRepository<T> : IRepository<T> where T : class
    {
    }

}
