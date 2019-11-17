using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Plato.Internal.Abstractions.Extensions;

namespace Plato.Internal.Data.Abstractions
{

    public class PagedResults<T> : IPagedResults<T> where T : class
    {

        private IList<T> _data;

        public IList<T> Data
        {
            get => _data ?? (_data = new List<T>());
            set => _data = value;
        }

        public int Total { get; set; }

        public T First()
        {
            return _data.First() ?? null;
        }

        public T FirstOrDefault()
        {
            return _data.First() ?? default;
        }

        public void PopulateTotal(IDataReader reader)
        {
            if (reader.ColumnIsNotNull(0))
            {
                Total = Convert.ToInt32(reader[0]);
            }                
        }

    }

}