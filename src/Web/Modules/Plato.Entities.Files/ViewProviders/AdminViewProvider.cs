using System;
using System.Linq;
using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using Plato.Files.Models;
using Plato.Files.Stores;
using Plato.Files.ViewModels;
using PlatoCore.Data.Abstractions;
using Plato.Entities.Files.Models;
using Plato.Entities.Files.Stores;
using Plato.Entities.Files.ViewModels;
using PlatoCore.Layout.ViewProviders.Abstractions;

namespace Plato.Entities.Files.ViewProviders
{
    public class AdminViewProvider : ViewProviderBase<File>
    {
        
        private readonly IEntityFileStore<EntityFile> _entityFileStore;
        private readonly IEntityStore<Entity> _entityStore;
        private readonly IFileStore<File> _fileStore;

        public AdminViewProvider(
            IEntityFileStore<EntityFile> entityfileStore,
            IEntityStore<Entity> entityStore,
            IFileStore<File> fileStore)
        {
            _entityFileStore = entityfileStore;
            _entityStore = entityStore;
            _fileStore = fileStore;
        }

        public override Task<IViewProviderResult> BuildIndexAsync(File file, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(File file, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(File file, IViewProviderContext context)
        {

            // No need to update the Create / Add view
            if (file.Id == 0)
            {
                return default(IViewProviderResult);
            }

            // Get entity relationships for file
            var relationships = await _entityFileStore.QueryAsync()
                .Take(int.MaxValue, false)
                .Select<EntityFileQueryParams>(q =>
                {
                    q.FileId.Equals(file.Id);
                })
                .ToList();

            // Get entities for file
            IPagedResults<Entity> entities = null;            
            if (relationships?.Data != null)
            {
                entities = await _entityStore.QueryAsync()
                  .Take(int.MaxValue, false)
                  .Select<EntityQueryParams>(q =>
                  {
                      q.Id.IsIn(relationships.Data.Select(f => f.EntityId).ToArray());
                  })
                  .ToList();
            }

            // Build view model
            var viewModel = new FileEntitiesViewModel()
            {
                Results = entities
            };

            // Return view
            return Views(                
                View<FileEntitiesViewModel>("Admin.Edit.FileEntities", model => viewModel)
                    .Zone("content-right").Order(5)
            );

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(File file, IViewProviderContext context)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            //if (file.IsNewFile)
            //{
            //    return await BuildEditAsync(file, context);
            //}

            var model = new EditFileViewModel();

            if (!await context.Updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(file, context);
            }

            model.Name = model.Name?.Trim();

            //model.Description = model.Description?.Trim();

            if (context.Updater.ModelState.IsValid)
            {

                file.Name = model.Name;
                //file.Description = model.Description;
                //file.ForeColor = model.ForeColor;
                //file.BackColor = model.BackColor;

                var updatedFile = await _fileStore.UpdateAsync(file);

                //foreach (var error in result.Errors)
                //{
                //    context.Updater.ModelState.AddModelError(string.Empty, error.Description);
                //}

            }

            return await BuildEditAsync(file, context);

        }

    }

}
