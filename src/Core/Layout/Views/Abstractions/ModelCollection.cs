using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace PlatoCore.Layout.Views.Abstractions
{
   
    public interface IModelCollection
    {

        void AddOrUpdate(object model);

        IReadOnlyDictionary<Type, IList<object>> Models { get; }

    }

    public class ModelCollection : IModelCollection
    {

        private readonly ConcurrentDictionary<Type, IList<object>> _models;

        public IReadOnlyDictionary<Type, IList<object>> Models => _models;

        public ModelCollection()
        {
            _models = new ConcurrentDictionary<Type, IList<object>>();
        }

        public void AddOrUpdate(object model)
        {
      
            if (model == null)
            {
                return;
            }

            _models.AddOrUpdate(model.GetType(), new List<object>()
            {
                model
            }, (k, v) =>
            {
                v.Add(model);
                return v;
            });

        }

    }

    public static class ModelCollectionExtensions
    {
        public static T FirstOf<T>(this IModelCollection viewModelCollection) where T : class
        {

            if (viewModelCollection.Models.ContainsKey(typeof(T)))
            {
                if (viewModelCollection.Models[typeof(T)][0] is T)
                {
                    return (T)viewModelCollection.Models[typeof(T)][0];
                }
            }

            return null;

        }
        public static IEnumerable<T> AllOf<T>(this IModelCollection viewModelCollection) where T : class
        {

            var output = new List<T>();
            if (viewModelCollection.Models.ContainsKey(typeof(T)))
            {
                foreach (var viewModel in viewModelCollection.Models[typeof(T)])
                {
                    if (viewModel is T)
                    {
                        output.Add((T)viewModel);
                    }
                }             
            }

            return output;

        }

    }

}
