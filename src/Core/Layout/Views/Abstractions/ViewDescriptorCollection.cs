using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace PlatoCore.Layout.Views.Abstractions
{

    public interface IViewDescriptorCollection
    {

        ViewDescriptor Add(ViewDescriptor descriptor);

        IReadOnlyDictionary<Type, IList<ViewDescriptor>> Descriptors { get; }

    }

    public class ViewDescriptorCollection : IViewDescriptorCollection
    {

        private readonly ConcurrentDictionary<Type, IList<ViewDescriptor>> _descriptors;

        public IReadOnlyDictionary<Type, IList<ViewDescriptor>> Descriptors => _descriptors;

        public ViewDescriptorCollection()
        {
            _descriptors = new ConcurrentDictionary<Type, IList<ViewDescriptor>>();
        }

        public ViewDescriptor Add(ViewDescriptor descriptor)
        {

            if (descriptor.View == null)
            {
                return descriptor;
            }

            if (descriptor.View.Model == null)
            {
                return descriptor;
            }

            _descriptors.AddOrUpdate(descriptor.View.Model.GetType(), new List<ViewDescriptor>()
            {
                descriptor
            }, (k, v) =>
            {
                v.Add(descriptor);
                return v;
            });

            return descriptor;
        }
       
    }

    public static class ViewDescriptorCollectionExtensions
    {
        public static T FirstDescriptorModelOfType<T>(this IViewDescriptorCollection viewDescriptorCollection) where T : class
        {

            if (viewDescriptorCollection.Descriptors.ContainsKey(typeof(T)))
            {
                if (viewDescriptorCollection.Descriptors[typeof(T)][0].View.Model is T)
                {
                    return (T)viewDescriptorCollection.Descriptors[typeof(T)][0].View.Model;
                }
            }

            return null;

        }

    }

}
