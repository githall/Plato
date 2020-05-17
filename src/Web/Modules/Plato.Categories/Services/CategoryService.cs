using System;
using System.Threading.Tasks;
using Plato.Categories.Models;
using Plato.Categories.Stores;
using Plato.Categories.ViewModels;
using PlatoCore.Data.Abstractions;
using PlatoCore.Hosting.Web.Abstractions;
using PlatoCore.Navigation.Abstractions;

namespace Plato.Categories.Services
{
   
    public class CategoryService<TModel> : ICategoryService<TModel> where TModel : class, ICategory
    {

        private Action<QueryOptions> _configureDb = null;
        private Action<CategoryQueryParams> _configureParams = null;

        private readonly ICategoryStore<TModel> _categoryStore;
        private readonly IContextFacade _contextFacade;
        
        public CategoryService(
            ICategoryStore<TModel> categoryStore,
            IContextFacade contextFacade)
        {
            _contextFacade = contextFacade;
            _categoryStore = categoryStore;

            // Default options delegate
            _configureDb = options => options.SearchType = SearchTypes.Tsql;

        }

        /// <summary>
        /// Optionally configure additional query options.
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public ICategoryService<TModel> ConfigureDb(Action<IQueryOptions> configure)
        {
            _configureDb = configure;
            return this;
        }

        /// <summary>
        /// Optionally configure query parameters to be used by GetResultAsync or GetResultsAsync.
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public ICategoryService<TModel> ConfigureQuery(Action<CategoryQueryParams> configure)
        {
            _configureParams = configure;
            return this;
        }

        /// <summary>
        /// Return a single category for the configured query.
        /// </summary>
        /// <returns></returns>
        public async Task<TModel> GetResultAsync()
        {

            // Get authenticated user 
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Return tailored results
            var results = await _categoryStore.QueryAsync()
                .Take(1, false)
                .Configure(_configureDb)
                .Select<CategoryQueryParams>(q =>
                {

                    // ----------------
                    // Set current authenticated user id
                    // This is required for various security checks
                    // ----------------

                    q.UserId.Equals(user?.Id ?? 0);

                    // ----------------
                    // Additional parameter configuration
                    // ----------------

                    _configureParams?.Invoke(q);

                })
                .ToList();

            if (results?.Data != null)
            {
                if (results.Data.Count > 0)
                {
                    return results.Data[0] as TModel;
                }                
            }

            return null;

        }

        /// <summary>
        /// Returns all possible categories for the configured query.
        /// </summary>
        /// <returns></returns>
        public Task<IPagedResults<TModel>> GetResultsAsync()
        {
            return GetResultsAsync(new CategoryIndexOptions(), new PagerOptions()
            {
                CountTotal = false,
                Size = int.MaxValue
            });
        }

        /// <summary>
        /// Return a paged set of results based on the supplied options & pager parameters plus the configured query if available. 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="pager"></param>
        /// <returns></returns>
        public async Task<IPagedResults<TModel>> GetResultsAsync(CategoryIndexOptions options, PagerOptions pager)
        {

            if (options == null)
            {
                options = new CategoryIndexOptions();
            }

            if (pager == null)
            {
                pager = new PagerOptions();
            }

            // Ensure we have a sort column is non is specified
            if (options.Sort == SortBy.Auto)
            {
                options.Sort = SortBy.SortOrder;
                options.Order = OrderBy.Asc;
            }

            // Get authenticated user 
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Return tailored results
            return await _categoryStore.QueryAsync()
                .Take(pager.Page, pager.Size, pager.CountTotal)                
                .Configure(_configureDb)
                .Select<CategoryQueryParams>(q =>
                {

                    // ----------------
                    // Set current authenticated user id
                    // This is required for various security checks
                    // ----------------
                    
                    q.UserId.Equals(user?.Id ?? 0);

                    // ----------------
                    // Basic parameters
                    // ----------------

                    // FeatureId
                    if (options.FeatureId > 0)
                    {
                        q.FeatureId.Equals(options.FeatureId);
                    }

                    // ----------------
                    // Additional parameter configuration
                    // ----------------

                    _configureParams?.Invoke(q);

                })
                .OrderBy(options.Sort.ToString(), options.Order)
                .ToList();

        }

    }

}
