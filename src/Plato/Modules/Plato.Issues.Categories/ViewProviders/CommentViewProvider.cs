using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Categories.Stores;
using Plato.Issues.Categories.Models;
using Plato.Issues.Models;
using Plato.Entities.Stores;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Issues.Categories.ViewProviders
{

    public class CommentViewProvider : BaseViewProvider<Comment>
    {
        
        private readonly ICategoryStore<Category> _categoryStore;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IEntityStore<Issue> _entityStore;

        public IStringLocalizer T;

        public IStringLocalizer S { get; }

        public CommentViewProvider(
            IStringLocalizer<CommentViewProvider> stringLocalizer,            
            ICategoryStore<Category> categoryStore, 
            IBreadCrumbManager breadCrumbManager,
            IEntityStore<Issue> entityStore)
        {

            _breadCrumbManager = breadCrumbManager;
            _categoryStore = categoryStore;
            _entityStore = entityStore;

            T = stringLocalizer;
            S = stringLocalizer;

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(Comment viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildIndexAsync(Comment viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(Comment viewModel, IViewProviderContext context)
        {


            var topic = await _entityStore.GetByIdAsync(viewModel.EntityId);
            if (topic == null)
            {
                return default(IViewProviderResult);
            }

            // Override breadcrumb configuration within base discuss controller 
            IEnumerable<CategoryAdmin> parents = null;
            if (topic.CategoryId > 0)
            {
                parents = await _categoryStore.GetParentsByIdAsync(topic.CategoryId);

            }

            _breadCrumbManager.Configure(builder =>
            {

                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Issues"], home => home
                    .Action("Index", "Home", "Plato.Issues")
                    .LocalNav()
                );

                if (parents != null)
                {
                    builder.Add(S["Categories"], categories => categories
                        .Action("Index", "Home", "Plato.Issues.Categories", new RouteValueDictionary()
                        {
                            ["opts.id"] = "",
                            ["opts.alias"] = ""
                        })
                        .LocalNav()
                    );
                    foreach (var parent in parents)
                    {
                        builder.Add(S[parent.Name], c => c
                            .Action("Index", "Home", "Plato.Issues.Categories", new RouteValueDictionary
                            {
                                ["opts.id"] = parent.Id,
                                ["opts.alias"] = parent.Alias,
                            })
                            .LocalNav()
                        );
                    }
                }

                builder.Add(S[topic.Title], t => t
                    .Action("Display", "Home", "Plato.Issues", new RouteValueDictionary
                    {
                        ["opts.id"] = topic.Id,
                        ["opts.alias"] = topic.Alias,
                    })
                    .LocalNav()
                );

                builder.Add(S["Edit Comment"]);

            });

            return default(IViewProviderResult);

        }

        public override Task<IViewProviderResult> BuildUpdateAsync(Comment viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

    }

}
