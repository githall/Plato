using System;
using System.Threading.Tasks;
using PlatoCore.Data.Abstractions;
using PlatoCore.Models.Users;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Stores.Users;
using Plato.Users.ViewModels;

namespace Plato.Users.Services
{

    public interface IUserService<TModel> where TModel : class, IUser
    {

        IUserService<TModel> ConfigureDb(Action<IQueryOptions> configure);

        Task<IPagedResults<TModel>> GetResultsAsync(UserIndexOptions options, PagerOptions pager);

        IUserService<TModel> ConfigureQuery(Action<UserQueryParams> configure);

    }

}
