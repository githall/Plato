using System;
using System.Threading.Tasks;
using Plato.Site.Models;
using Plato.Site.Stores;
using PlatoCore.Abstractions;
using PlatoCore.Messaging.Abstractions;
using PlatoCore.Text.Abstractions;

namespace Plato.Site.Services
{

    public class SignUpManager : ISignUpManager<SignUp>
    {

        private readonly ISignUpValidator _companyNameValidator;

        private readonly ISignUpStore<SignUp> _signUpStore;
        private readonly IKeyGenerator _keyGenerator;
        private readonly IAliasCreator _aliasCreator;
        private readonly IBroker _broker;

        public SignUpManager(
            ISignUpValidator companyNameValidator,
            ISignUpStore<SignUp> signUpStore,
            IKeyGenerator keyGenerator,
            IAliasCreator aliasCreator,
            IBroker broker)
        {
            _companyNameValidator = companyNameValidator;
            _keyGenerator = keyGenerator;
            _aliasCreator = aliasCreator;
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

            // Create security token
            if (string.IsNullOrEmpty(model.SecurityToken))
            {
                // Build simple security token 
                model.SecurityToken = _keyGenerator.GenerateKey(o =>
                {
                    o.OnlyDigits = true;
                    o.MaxLength = 6;
                });
            }

            // Do we have a company name?
            if (!string.IsNullOrEmpty(model.CompanyName))
            {

                // Validate company name
                var validationResult = await _companyNameValidator.ValidateCompanyNameAsync(model.CompanyName);
                if (!validationResult.Succeeded)
                {
                    return result.Failed(validationResult.Errors.ToString());
                }

                // Create company name alias
                model.CompanyNameAlias = _aliasCreator.Create(model.CompanyName);
            }

            // Invoke SignUpCreating subscriptions
            foreach (var handler in _broker.Pub<SignUp>(this, "SignUpCreating"))
            {
                model = await handler.Invoke(new Message<SignUp>(model, this));
            }

            var signUp = await _signUpStore.CreateAsync(model);
            if (signUp != null)
            {

                // Set unique session id, using primary key to ensure uniqueness
                if (string.IsNullOrEmpty(signUp.SessionId))
                {                  

                    // Set sessionId
                    signUp.SessionId = _keyGenerator.GenerateKey(o =>
                    {
                        o.OnlyDigits = false;
                        o.UniqueIdentifier = signUp.Id.ToString();
                        o.MaxLength = 150;
                    });

                    // Persist sessionId
                    var updatedSignUp = await _signUpStore.UpdateAsync(signUp);
                    if (updatedSignUp != null)
                    {
                        // Invoke SignUpCreated subscriptions
                        foreach (var handler in _broker.Pub<SignUp>(this, "SignUpCreated"))
                        {
                            updatedSignUp = await handler.Invoke(new Message<SignUp>(updatedSignUp, this));
                        }

                        // Return success
                        return result.Success(updatedSignUp);

                    }

                }

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




            // Create company name alias
            if (string.IsNullOrEmpty(model.CompanyName))
            {
                model.CompanyNameAlias = _aliasCreator.Create(model.CompanyName);
            }

            // Invoke SignUpUpdating subscriptions
            foreach (var handler in _broker.Pub<SignUp>(this, "SignUpUpdating"))
            {
                model = await handler.Invoke(new Message<SignUp>(model, this));
            }

            var signUp = await _signUpStore.UpdateAsync(model);
            if (signUp != null)
            {

                // Invoke SignUpUpdated subscriptions
                foreach (var handler in _broker.Pub<SignUp>(this, "SignUpUpdated"))
                {
                    model = await handler.Invoke(new Message<SignUp>(model, this));
                }

                // Return success
                return result.Success(signUp);

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
