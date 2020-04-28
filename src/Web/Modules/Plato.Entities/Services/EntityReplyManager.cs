﻿using System;
using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using PlatoCore.Abstractions;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Hosting.Web.Abstractions;
using PlatoCore.Messaging.Abstractions;

namespace Plato.Entities.Services
{
    public class EntityReplyManager<TReply> : IEntityReplyManager<TReply> where TReply : class, IEntityReply
    {

        public event EntityReplyEvents<TReply>.Handler Creating;
        public event EntityReplyEvents<TReply>.Handler Created;
        public event EntityReplyEvents<TReply>.Handler Updating;
        public event EntityReplyEvents<TReply>.Handler Updated;
        public event EntityReplyEvents<TReply>.Handler Deleting;
        public event EntityReplyEvents<TReply>.Handler Deleted;
     
        private readonly IBroker _broker;
        private readonly IEntityStore<Entity> _entityStore;
        private readonly IEntityReplyStore<TReply> _entityReplyStore;
        private readonly IContextFacade _contextFacade;
 
        public EntityReplyManager(
            IEntityReplyStore<TReply> entityReplyStore, 
            IBroker broker, IContextFacade contextFacade,
            IEntityStore<Entity> entityStore)
        {
            _entityReplyStore = entityReplyStore;
            _contextFacade = contextFacade;
            _entityStore = entityStore;
            _broker = broker;
        }

        public async Task<ICommandResult<TReply>> CreateAsync(TReply reply)
        {

            // Validate
            if (reply == null)
            {
                throw new ArgumentNullException(nameof(reply));
            }

            if (reply.Id > 0)
            {
                throw new ArgumentOutOfRangeException(nameof(reply.Id));
            }

            if (reply.EntityId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(reply.EntityId));
            }
            
            if (String.IsNullOrWhiteSpace(reply.Message))
            {
                throw new ArgumentNullException(nameof(reply.Message));
            }
            
            if (reply.CreatedDate == null)
            {
                throw new ArgumentNullException(nameof(reply.CreatedDate));
            }

            var result = new CommandResult<TReply>();

            // Ensure the entity exists
            var entity = await _entityStore.GetByIdAsync(reply.EntityId);
            if (entity == null)
            {
                return result.Failed(new CommandError($"An entity with the Id '{reply.EntityId}' could not be found"));
            }

            // Parse Html and abstract
            reply.Html = await ParseEntityHtml(reply.Message);
            reply.Abstract = await ParseEntityAbstract(reply.Message);
            reply.Urls = await ParseEntityUrls(reply.Html);

            // Raise creating event
            Creating?.Invoke(this, new EntityReplyEventArgs<TReply>(entity, reply));

            // Invoke EntityReplyCreating subscriptions
            foreach (var handler in _broker.Pub<TReply>(this, "EntityReplyCreating"))
            {
                reply = await handler.Invoke(new Message<TReply>(reply, this));
            }

            var newReply = await _entityReplyStore.CreateAsync(reply);
            if (newReply != null)
            {
                // Raise created event
                Created?.Invoke(this, new EntityReplyEventArgs<TReply>(entity, newReply));

                // Invoke EntityReplyCreated subscriptions
                foreach (var handler in _broker.Pub<TReply>(this, "EntityReplyCreated"))
                {
                    newReply = await handler.Invoke(new Message<TReply>(newReply, this));
                }
                
                return result.Success(newReply);
            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to create the reply"));

        }

        public async Task<ICommandResult<TReply>> UpdateAsync(TReply reply)
        {

            // Validate
            if (reply == null)
            {
                throw new ArgumentNullException(nameof(reply));
            }
            
            if (reply.Id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(reply.Id));
            }
            
            if (reply.EntityId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(reply.EntityId));
            }
            
            if (String.IsNullOrWhiteSpace(reply.Message))
            {
                throw new ArgumentNullException(nameof(reply.Message));
            }

            //if (reply.ModifiedUserId <= 0)
            //{
            //    throw new ArgumentOutOfRangeException(nameof(reply.ModifiedUserId));
            //}
            
            //if (reply.ModifiedDate == null)
            //{
            //    throw new ArgumentNullException(nameof(reply.ModifiedDate));
            //}
            
            var result = new CommandResult<TReply>();

            // Ensure entity exists
            var entity = await _entityStore.GetByIdAsync(reply.EntityId);
            if (entity == null)
            {
                return result.Failed(new CommandError($"An entity with the Id '{reply.EntityId}' could not be found"));
            }
            
            // Parse Html and message abstract
            reply.Html = await ParseEntityHtml(reply.Message);
            reply.Abstract = await ParseEntityAbstract(reply.Message);
            reply.Urls = await ParseEntityUrls(reply.Html);

            // Raise updating event
            Updating?.Invoke(this, new EntityReplyEventArgs<TReply>(entity, reply));

            // Invoke EntityReplyUpdating subscriptions
            foreach (var handler in _broker.Pub<TReply>(this, "EntityReplyUpdating"))
            {
                reply = await handler.Invoke(new Message<TReply>(reply, this));
            }

            var updatedReply = await _entityReplyStore.UpdateAsync(reply);
            if (updatedReply != null)
            {
                // Raise Updated event
                Updated?.Invoke(this, new EntityReplyEventArgs<TReply>(entity, updatedReply));

                // Invoke EntityReplyUpdated subscriptions
                foreach (var handler in _broker.Pub<TReply>(this, "EntityReplyUpdated"))
                {
                    updatedReply = await handler.Invoke(new Message<TReply>(updatedReply, this));
                }

                return result.Success(updatedReply);
            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to update the reply."));

        }

        public async Task<ICommandResult<TReply>> DeleteAsync(TReply reply)
        {

            // Validate
            if (reply == null)
            {
                throw new ArgumentNullException(nameof(reply));
            }
            
            var result = new CommandResult<TReply>();
            
            // Raise Deleting event
            Deleting?.Invoke(this, new EntityReplyEventArgs<TReply>(null, reply));

            // Invoke EntityReplyDeleting subscriptions
            foreach (var handler in _broker.Pub<TReply>(this, "EntityReplyDeleting"))
            {
                reply = await handler.Invoke(new Message<TReply>(reply, this));
            }
        
            var success = await _entityReplyStore.DeleteAsync(reply);
            if (success)
            {

                // Raise Deleted event
                Deleted?.Invoke(this, new EntityReplyEventArgs<TReply>(null, reply));

                // Invoke EntityReplyDeleted subscriptions
                foreach (var handler in _broker.Pub<TReply>(this, "EntityReplyDeleted"))
                {
                    reply = await handler.Invoke(new Message<TReply>(reply, this));
                }
       
                return result.Success(reply);
            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to delete the reply."));

        }
        
        #region "Private Methods"

        private async Task<string> ParseEntityHtml(string message)
        {
            
            foreach (var handler in _broker.Pub<string>(this, "ParseEntityHtml"))
            {
                message = await handler.Invoke(new Message<string>(message, this));
            }

            return message;

        }

        private async Task<string> ParseEntityAbstract(string message)
        {
            
            foreach (var handler in _broker.Pub<string>(this, "ParseEntityAbstract"))
            {
                message = await handler.Invoke(new Message<string>(message, this));
            }

            return message.PlainTextulize().TrimToAround(225);

        }

        async Task<string> ParseEntityUrls(string html)
        {

            var handled = false;
            foreach (var handler in _broker.Pub<string>(this, "ParseEntityUrls"))
            {
                handled = true;
                html = await handler.Invoke(new Message<string>(html, this));
            }

            return handled ? html : string.Empty;

        }

        #endregion


    }
}
