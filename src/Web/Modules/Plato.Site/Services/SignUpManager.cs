using System;
using System.Threading.Tasks;
using Plato.Site.Models;
using Plato.Site.Stores;
using PlatoCore.Abstractions;
using PlatoCore.Messaging.Abstractions;

namespace Plato.Site.Services
{

    public class SignUpManager : ISignUpManager<SignUp>
    {

        private readonly ISignUpStore<SignUp> _signUpStore;
        private readonly IBroker _broker;

        public SignUpManager(
            ISignUpStore<SignUp> signUpStore,
            IBroker broker)
        {
            _signUpStore = signUpStore;
            _broker = broker;
        }

        public async Task<ICommandResult<SignUp>> CreateAsync(SignUp model)
        {

            var result = new CommandResult<SignUp>();

            // Validate
            if (model.Id > 0)
            {
                return result.Failed(new CommandError($"{nameof(model.Id)} cannot be greater than zero when creating a sign-up"));
            }

            if (string.IsNullOrEmpty(model.Email))
            {
                throw new ArgumentOutOfRangeException(nameof(model.Email));
            }

            // Invoke SignUpCreating subscriptions
            foreach (var handler in _broker.Pub<SignUp>(this, "SignUpCreating"))
            {
                model = await handler.Invoke(new Message<SignUp>(model, this));
            }

            var entity = await _signUpStore.CreateAsync(model);
            if (entity != null)
            {

                // Invoke SignUpCreating subscriptions
                foreach (var handler in _broker.Pub<SignUp>(this, "SignUpCreating"))
                {
                    model = await handler.Invoke(new Message<SignUp>(model, this));
                }

                // Return success
                return result.Success(entity);

            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to create a sign-up"));


        }

        public async Task<ICommandResult<SignUp>> UpdateAsync(SignUp model)
        {
            var result = new CommandResult<SignUp>();

            // Validate
            if (model.Id <= 0)
            {
                return result.Failed(new CommandError($"{nameof(model.Id)} cannot be zero or less than zero when updating a sign-up"));
            }

            if (string.IsNullOrEmpty(model.Email))
            {
                throw new ArgumentOutOfRangeException(nameof(model.Email));
            }

            // Invoke SignUpUpdating subscriptions
            foreach (var handler in _broker.Pub<SignUp>(this, "SignUpUpdating"))
            {
                model = await handler.Invoke(new Message<SignUp>(model, this));
            }

            var entity = await _signUpStore.UpdateAsync(model);
            if (entity != null)
            {

                // Invoke SignUpUpdated subscriptions
                foreach (var handler in _broker.Pub<SignUp>(this, "SignUpUpdated"))
                {
                    model = await handler.Invoke(new Message<SignUp>(model, this));
                }

                // Return success
                return result.Success(entity);

            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to create a sign-up"));

        }

        public async Task<ICommandResult<SignUp>> DeleteAsync(SignUp model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var result = new CommandResult<SignUp>();

            var signUp = await _signUpStore.GetByIdAsync(model.Id);
            if (signUp == null)
            {
                return result.Failed(new CommandError($"A sign-up with the id {model.Id} could not be found"));
            }

            // Invoke SignUpDeleting subscriptions
            foreach (var handler in _broker.Pub<SignUp>(this, "SignUpDeleting"))
            {
                signUp = await handler.Invoke(new Message<SignUp>(signUp, this));
            }

            var success = await _signUpStore.DeleteAsync(signUp);
            if (success)
            {

                // Invoke SignUpDeleted subscriptions
                foreach (var handler in _broker.Pub<SignUp>(this, "SignUpDeleted"))
                {
                    signUp = await handler.Invoke(new Message<SignUp>(signUp, this));
                }

                return result.Success(signUp);
            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to delete a sign-up."));

        }

    }

}
