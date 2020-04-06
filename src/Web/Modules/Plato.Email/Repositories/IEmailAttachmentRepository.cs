using PlatoCore.Repositories;

namespace Plato.Email.Repositories
{

    public interface IEmailAttachmentRepository<T> : IRepository<T> where T : class
    {
    }

}
