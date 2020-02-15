using System;
using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;
using PlatoCore.Data.Abstractions;
using PlatoCore.Navigation.Abstractions;

namespace Plato.Entities.Services
{

    public interface ISimpleEntityService<TModel> where TModel : class, ISimpleEntity
    {

        ISimpleEntityService<TModel> ConfigureDb(Action<IQueryOptions> configure);

        ISimpleEntityService<TModel> ConfigureQuery(Action<SimpleEntityQueryParams> configure);

        Task<IPagedResults<TModel>> GetResultsAsync();

        Task<IPagedResults<TModel>> GetResultsAsync(EntityIndexOptions options);

        Task<IPagedResults<TModel>> GetResultsAsync(EntityIndexOptions options, PagerOptions pager);

    }

    public interface IEntityService<TModel> where TModel : class, IEntity
    {

        IEntityService<TModel> ConfigureDb(Action<IQueryOptions> configure);

        IEntityService<TModel> ConfigureQuery(Action<EntityQueryParams> configure);

        Task<IPagedResults<TModel>> GetResultsAsync();

        Task<IPagedResults<TModel>> GetResultsAsync(EntityIndexOptions options);

        Task<IPagedResults<TModel>> GetResultsAsync(EntityIndexOptions options, PagerOptions pager);

    }

}
