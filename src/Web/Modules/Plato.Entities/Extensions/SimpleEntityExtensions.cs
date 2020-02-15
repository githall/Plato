using System;
using System.Linq;
using System.Collections.Generic;
using Plato.Entities.Models;

namespace Plato.Entities.Extensions
{

    public static class SimpleEntityExtensions
    {

        public static IEnumerable<TEntity> BuildHierarchy<TEntity>(this IEnumerable<ISimpleEntity> input) where TEntity : class, ISimpleEntity
        {
            return BuildHierarchyRecursively<TEntity>(input.ToLookup(c => c.ParentId));
        }

        public static IEnumerable<TEntity> RecurseParents<TEntity>(this IEnumerable<ISimpleEntity> input, int id) where TEntity : class, ISimpleEntity
        {
            return RecurseParentsInternal<TEntity>(input.ToList(), id);
        }

        public static IEnumerable<TEntity> RecurseChildren<TEntity>(this IEnumerable<ISimpleEntity> input, int id) where TEntity : class, ISimpleEntity
        {
            return RecurseChildrenInternal<TEntity>(input.ToList(), id);
        }

        // -----------------

        private static IList<T> BuildHierarchyRecursively<T>(
           ILookup<int, ISimpleEntity> input,
           IList<T> output = null,
           ISimpleEntity parent = null,
           int parentId = 0,
           int depth = 0) where T : class, ISimpleEntity
        {

            if (input == null) throw new ArgumentNullException(nameof(input));
            if (output == null) output = new List<T>();
            if (parentId == 0) depth = 0;

            foreach (var entity in input[parentId])
            {

                var item = (T)entity;
                if (depth < 0) depth = 0;
                if (parent != null) depth++;

                item.Depth = depth;
                item.Parent = parent;

                if (parent != null)
                {
                    var children = new List<ISimpleEntity>() { item };
                    if (parent.Children != null)
                    {
                        children.AddRange(parent.Children);
                    }

                    parent.Children = children.OrderBy(c => c.SortOrder);
                }

                output.Add(item);

                // recurse
                BuildHierarchyRecursively<T>(input, output, item, item.Id, depth--);

            }

            return output;

        }

        private static IEnumerable<T> RecurseParentsInternal<T>(
            IList<ISimpleEntity> input,
            int rootId,
            IList<T> output = null) where T : class, ISimpleEntity
        {
            if (output == null)
            {
                output = new List<T>();
            }

            foreach (var entity in input)
            {
                var item = (T)entity;
                if (item.Id == rootId)
                {
                    if (item.ParentId > 0)
                    {
                        output.Add(item);
                        RecurseParentsInternal(input, item.ParentId, output);
                    }
                    else
                    {
                        output.Add(item);
                    }
                }
            }

            return output;

        }

        private static IEnumerable<T> RecurseChildrenInternal<T>(
            IList<ISimpleEntity> input,
            int rootId,
            IList<T> output = null) where T : class, ISimpleEntity
        {

            if (output == null)
            {
                output = new List<T>();
            }

            foreach (var entity in input)
            {
                var item = (T)entity;
                if (item.ParentId == rootId)
                {
                    output.Add(item);
                    RecurseChildrenInternal(input, item.Id, output);
                }
            }

            return output;

        }

    }

}
