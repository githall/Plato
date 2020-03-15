using System;
using System.Threading.Tasks;
using Plato.Files.Models;
using Plato.Files.Stores;
using PlatoCore.Abstractions;
using PlatoCore.Messaging.Abstractions;
using PlatoCore.Text.Abstractions;

namespace Plato.Files.Services
{

    public class FileManager : IFileManager
    {
        
        private readonly IFileStore<File> _fileStore;
        private readonly IAliasCreator _aliasCreator;    
        private readonly IBroker _broker;

        public FileManager(
            IFileStore<File> fileStore,      
            IAliasCreator aliasCreator,
            IBroker broker)
        {
            _aliasCreator = aliasCreator;
            _fileStore = fileStore;
            _broker = broker;
        }

        #region "Implementation"

        public async Task<ICommandResult<File>> CreateAsync(File model)
        {

            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.FeatureId <= 0)
            {
                throw new ArgumentNullException(nameof(model.FeatureId));
            }

            if (model.Id > 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }

            if (String.IsNullOrWhiteSpace(model.Name))
            {
                throw new ArgumentNullException(nameof(model.Name));
            }

            if (model.CreatedUserId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.CreatedUserId));
            }

            if (model.CreatedDate == null)
            {
                throw new ArgumentNullException(nameof(model.CreatedDate));
            }

            model.Alias = await ParseAlias(model.Name);

            // Invoke FileCreating subscriptions
            foreach (var handler in _broker.Pub<File>(this, "FileCreating"))
            {
                model = await handler.Invoke(new Message<File>(model, this));
            }

            var result = new CommandResult<File>();

            var newFile = await _fileStore.CreateAsync(model);
            if (newFile != null)
            {

                // Invoke FileCreated subscriptions
                foreach (var handler in _broker.Pub<File>(this, "FileCreated"))
                {
                    newFile = await handler.Invoke(new Message<File>(newFile, this));
                }

                // Return success
                return result.Success(newFile);

            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to create the file"));

        }

        public async Task<ICommandResult<File>> UpdateAsync(File model)
        {

            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.FeatureId <= 0)
            {
                throw new ArgumentNullException(nameof(model.FeatureId));
            }

            if (model.Id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }

            if (String.IsNullOrWhiteSpace(model.Name))
            {
                throw new ArgumentNullException(nameof(model.Name));
            }

            if (model.CreatedUserId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.CreatedUserId));
            }

            if (model.CreatedDate == null)
            {
                throw new ArgumentNullException(nameof(model.CreatedDate));
            }

            // Configure model

            model.Alias = await ParseAlias(model.Name);

            // Invoke FileUpdating subscriptions
            foreach (var handler in _broker.Pub<File>(this, "FileUpdating"))
            {
                model = await handler.Invoke(new Message<File>(model, this));
            }

            var result = new CommandResult<File>();

            var updatedFile = await _fileStore.UpdateAsync(model);
            if (updatedFile != null)
            {

                // Invoke FileUpdated subscriptions
                foreach (var handler in _broker.Pub<File>(this, "FileUpdated"))
                {
                    updatedFile = await handler.Invoke(new Message<File>(updatedFile, this));
                }

                // Return success
                return result.Success(updatedFile);
            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to update the file"));

        }

        public async Task<ICommandResult<File>> DeleteAsync(File model)
        {

            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            // Invoke FileDeleting subscriptions
            foreach (var handler in _broker.Pub<File>(this, "FileDeleting"))
            {
                model = await handler.Invoke(new Message<File>(model, this));
            }

            var result = new CommandResult<File>();
            if (await _fileStore.DeleteAsync(model))
            {

                // Invoke FileDeleted subscriptions
                foreach (var handler in _broker.Pub<File>(this, "FileDeleted"))
                {
                    model = await handler.Invoke(new Message<File>(model, this));
                }

                // Return success
                return result.Success(model);

            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to delete the file"));

        }

        #endregion

        #region "Private Methods"

        private async Task<string> ParseAlias(string input)
        {

            var handled = false;
            foreach (var handler in _broker.Pub<string>(this, "ParseFileAlias"))
            {
                handled = true;
                input = await handler.Invoke(new Message<string>(input, this));
            }

            // No subscription found, use default alias creator
            return handled ? input : _aliasCreator.Create(input);

        }

        #endregion

    }

}
