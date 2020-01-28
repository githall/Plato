using System;

namespace PlatoCore.Repositories.Users
{
    public interface IUserSecretRepository<T> : IRepository<T> where T : class
    {
    }
}
