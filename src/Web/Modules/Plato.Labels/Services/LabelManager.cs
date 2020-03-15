using System;
using System.Threading.Tasks;
using PlatoCore.Abstractions;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Messaging.Abstractions;
using PlatoCore.Stores.Abstractions.Roles;
using PlatoCore.Text.Abstractions;
using Plato.Labels.Models;
using Plato.Labels.Stores;

namespace Plato.Labels.Services
{

    public class LabelManager<TLabel> : ILabelManager<TLabel> where TLabel : class, ILabel
    {

        private readonly ILabelDataStore<LabelData> _labelDataStore;
        private readonly ILabelStore<TLabel> _labelStore;
        private readonly IContextFacade _contextFacade;
        private readonly IAliasCreator _aliasCreator;
        private readonly IPlatoRoleStore _roleStore;
        private readonly IBroker _broker;

        public LabelManager(
            ILabelStore<TLabel> labelStore,
            //ILabelRoleStore<LabelRole> labelRoleStore,
            ILabelDataStore<LabelData> labelDataStore,
            IContextFacade contextFacade,
            IAliasCreator aliasCreator,
            IPlatoRoleStore roleStore,
            IBroker broker)
        {
            _labelDataStore = labelDataStore;
            _contextFacade = contextFacade;
            _aliasCreator = aliasCreator;
            _labelStore = labelStore;
            _roleStore = roleStore;
            _broker = broker;
        }

        #region "Implementation"

        public async Task<ICommandResult<TLabel>> CreateAsync(TLabel model)
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
          
            // Invoke LabelCreating subscriptions
            foreach (var handler in _broker.Pub<TLabel>(this,"LabelCreating"))
            {
                model = await handler.Invoke(new Message<TLabel>(model, this));
            }

            var result = new CommandResult<TLabel>();

            var label = await _labelStore.CreateAsync(model);
            if (label != null)
            {
           
                // Invoke LabelCreated subscriptions
                foreach (var handler in _broker.Pub<TLabel>(this, "LabelCreated"))
                {
                    label = await handler.Invoke(new Message<TLabel>(label, this));
                }

                // Return success
                return result.Success(label);

            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to create the Label"));
            
        }

        public async Task<ICommandResult<TLabel>> UpdateAsync(TLabel model)
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
            
            // Invoke LabelUpdating subscriptions
            foreach (var handler in _broker.Pub<TLabel>(this, "LabelUpdating"))
            {
                model = await handler.Invoke(new Message<TLabel>(model, this));
            }

            var result = new CommandResult<TLabel>();

            var label = await _labelStore.UpdateAsync(model);
            if (label != null)
            {

                // Invoke LabelUpdated subscriptions
                foreach (var handler in _broker.Pub<TLabel>(this, "LabelUpdated"))
                {
                    label = await handler.Invoke(new Message<TLabel>(label, this));
                }

                // Return success
                return result.Success(label);
            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to update the Label"));
            
        }

        public async Task<ICommandResult<TLabel>> DeleteAsync(TLabel model)
        {

            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            
            // Invoke LabelDeleting subscriptions
            foreach (var handler in _broker.Pub<TLabel>(this, "LabelDeleting"))
            {
                model = await handler.Invoke(new Message<TLabel>(model, this));
            }
            
            var result = new CommandResult<TLabel>();
            if (await _labelStore.DeleteAsync(model))
            {
                // Delete Label roles
                //await _labelRoleStore.DeleteByLabelIdAsync(model.Id);

                // Delete Label data
                var data = await _labelDataStore.GetByLabelIdAsync(model.Id);
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        await _labelDataStore.DeleteAsync(item);
                    }
                }
                
                // Invoke LabelDeleted subscriptions
                foreach (var handler in _broker.Pub<TLabel>(this, "LabelDeleted"))
                {
                    model = await handler.Invoke(new Message<TLabel>(model, this));
                }

                // Return success
                return result.Success();

            }
            
            return result.Failed(new CommandError("An unknown error occurred whilst attempting to delete the Label"));

        }

        #endregion

        #region "Private Methods"

        private async Task<string> ParseAlias(string input)
        {

            var handled = false;
            foreach (var handler in _broker.Pub<string>(this, "ParseLabelAlias"))
            {
                handled = true;
                input =  await handler.Invoke(new Message<string>(input, this));
            }

            // No subscription found, use default alias creator
            return handled ? input :  _aliasCreator.Create(input);

        }

        #endregion

    }

}
