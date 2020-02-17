using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlatoCore.Data.Abstractions
{

    public abstract class DefaultQuery<TModel> : IQuery<TModel> where TModel : class
    {
        
        private readonly QueryOptions _options;

        private readonly Dictionary<string, OrderBy> _sortColumns;
        
        public IQueryOptions Options => _options;
   
        public IDictionary<string, OrderBy> SortColumns => _sortColumns;
        
        public int PageIndex { get; private set; } = 1;

        public int PageSize { get; private set; } = int.MaxValue;

        /// <summary>
        /// Returns true if no page size has been specified.
        /// </summary>
        public bool IsDefaultPageSize => PageSize == int.MaxValue ? true : false;

        /// <summary>
        /// Indicates if a total count should be returned with the query. The default is true.
        /// </summary>
        public bool CountTotal { get; private set; } = true;

        public IQuery<TModel> Take(int pageSize, bool countTotal = true)
        {
            PageIndex = 1;
            PageSize = pageSize;
            CountTotal = countTotal;
            return this;
        }

        public IQuery<TModel> Take(int pageIndex, int pageSize, bool countTotal = true)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            CountTotal = countTotal;
            return this;
        }

        public IQuery<TModel> Configure(Action<QueryOptions> configure)
        {
            configure(_options);
            return this;
        }

        public abstract IQuery<TModel> Select<T>(Action<T> configure) where T : new();
        
        public abstract Task<IPagedResults<TModel>> ToList();

        public IQuery<TModel> OrderBy(IDictionary<string, OrderBy> columns)
        {
            foreach (var column in columns)
            {
                OrderBy(column.Key, column.Value);
            }

            return this;
        }

        public IQuery<TModel> OrderBy(string columnName, OrderBy sortOrder = Abstractions.OrderBy.Asc)
        {
            // We always need a key
            if (!string.IsNullOrEmpty(columnName) && !_sortColumns.ContainsKey(columnName))
            {
                _sortColumns.Add(columnName, sortOrder);
            }

            return this;
        }

        protected DefaultQuery()
        {
            _sortColumns = new Dictionary<string, OrderBy>();
            _options = new QueryOptions();
        }

    }

}