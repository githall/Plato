using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using PlatoCore.Layout.Views.Abstractions;

namespace PlatoCore.Layout.Views
{

    public interface IViewResultTable
    {
        void Add(object result);

        IReadOnlyDictionary<Type, IList<object>> Results { get; }

    }

    public class ViewResultTable : IViewResultTable
    {

        private readonly ConcurrentDictionary<Type, IList<object>> _results;

        public IReadOnlyDictionary<Type, IList<object>> Results => _results;

        public ViewResultTable()
        {
            _results = new ConcurrentDictionary<Type, IList<object>>();
        }

        public void Add(object result)
        {

            if (result == null)
            {
                return;
            }

            _results.AddOrUpdate(result.GetType(), new List<object>()
            {
                result
            }, (k, v) =>
            {
                v.Add(result);
                return v;
            });

        }
       
    }

    public static class ViewResultsTableManagerExtensions
    {
        public static IEnumerable<T> ViewResultsOfType<T>(this IViewResultTable viewResultTable) where T : class
        {

            IList<T> output = null;
            if (viewResultTable.Results.ContainsKey(typeof(T)))
            {
                var results = viewResultTable.Results[typeof(T)];
                foreach (var result in results)
                {
                    if (result is T)
                    {
                        if (output == null)
                        {
                            output = new List<T>();
                        }
                        output.Add((T)result);
                    }
                }
            }

            return output;

        }

        public static T FirstViewComponentWithType<T>(this IViewResultTable viewResultTable) where T : class
        {

            var viewComponentResults = viewResultTable.ViewResultsOfType<ViewViewComponentResult>();
            foreach (var viewComponentResult in viewComponentResults)
            {
                if (viewComponentResult.ViewData.Model is T)
                {
                    return (T)viewComponentResult.ViewData.Model;
                }
            }

            return null;

        }
}

}
