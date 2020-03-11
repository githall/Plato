using System;
using System.Collections.Generic;
using Plato.Attachments.Models;
using PlatoCore.Data.Abstractions;
using PlatoCore.Navigation.Abstractions;

namespace Plato.Attachments.ViewModels
{

    public class AttachmentIndexViewModel
    {

        public IPagedResults<Attachment> Results { get; set; }

        public PagerOptions Pager { get; set; }

        public AttachmentIndexOptions Options { get; set; }

        public ICollection<SortColumn> SortColumns { get; set; }

        public ICollection<SortOrder> SortOrder { get; set; }

        public ICollection<Filter> Filters { get; set; }

    }

    public class AttachmentIndexOptions
    {

        public FilterBy Filter { get; set; } = FilterBy.All;

        public int FeatureId { get; set; }

        public string Search { get; set; }

        public SortBy Sort { get; set; } = SortBy.Created;

        public OrderBy Order { get; set; } = OrderBy.Desc;

    }

    public class SortColumn
    {
        public string Text { get; set; }

        public SortBy Value { get; set; }

    }

    public class SortOrder
    {
        public string Text { get; set; }

        public OrderBy Value { get; set; }

    }

    public class Filter
    {
        public string Text { get; set; }

        public FilterBy Value { get; set; }

    }

    public enum SortBy
    {
        Id = 1,
        FeatureId = 2,
        Name = 3,
        Type = 4,
        Size = 5,
        Uniqueness = 6,
        Views = 7,
        CreatedUserId = 8,
        Created = 9,
    }

    public enum FilterBy
    {
        All = 0,
        Started = 1,
        Participated = 2,
        Following = 3,
        Starred = 4,
        Unanswered = 5,
        NoReplies = 6
    }
}
