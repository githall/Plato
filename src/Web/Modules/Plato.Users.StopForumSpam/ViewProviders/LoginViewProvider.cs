﻿using System.Threading.Tasks;
using PlatoCore.Layout.ModelBinding;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Models.Users;
using PlatoCore.Http.Abstractions;
using PlatoCore.Stores.Abstractions.Users;
using Plato.StopForumSpam.Services;

namespace Plato.Users.StopForumSpam.ViewProviders
{
    public class LoginViewProvider : ViewProviderBase<LoginPage>
    {

        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly ISpamOperatorManager<User> _spamOperatorManager;
        private readonly IClientIpAddress _clientIpAddress;

        public LoginViewProvider(
            IClientIpAddress clientIpAddress, 
            ISpamOperatorManager<User> spamOperatorManager,
            IPlatoUserStore<User> platoUserStore)
        {
            _clientIpAddress = clientIpAddress;
            _spamOperatorManager = spamOperatorManager;
            _platoUserStore = platoUserStore;
        }
        
        public override Task<IViewProviderResult> BuildIndexAsync(LoginPage viewModel,
            IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildDisplayAsync(LoginPage viewModel,
            IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(LoginPage viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<bool> ValidateModelAsync(LoginPage userLogin, IUpdateModel updater)
        {

            // Build user to validate
            var user = await BuildUserAsync(userLogin);

            // Could not build user details from supplied username
            if (user == null)
            {
                return true;
            }

            // Validate model within registered spam operators
            var results = await _spamOperatorManager.ValidateModelAsync(SpamOperations.Login, user);

            // IF any operators failed ensure we display the operator error message
            var valid = true;
            if (results != null)
            {
                foreach (var result in results)
                {
                    if (!result.Succeeded)
                    {
                        if (result.Operation.CustomMessage)
                        {
                            updater.ModelState.AddModelError(string.Empty,
                                !string.IsNullOrEmpty(result.Operation.Message)
                                    ? result.Operation.Message
                                    : $"Sorry but we've identified your details have been used by known spammers.");
                            valid = false;
                        }
                    }
                }
            }

            return valid;

        }
        
        public override async Task<IViewProviderResult> BuildUpdateAsync(LoginPage userLogin, IViewProviderContext context)
        {

            if (!context.Updater.ModelState.IsValid)
            {
                return await BuildIndexAsync(userLogin, context);
            }
            
            // Build user to validate
            var user = await BuildUserAsync(userLogin);

            // Could not build user details from supplied username
            if (user == null)
            {
                return await BuildIndexAsync(userLogin, context);
            }

            // Execute UpdateModel within registered spam operators
            await _spamOperatorManager.UpdateModelAsync(SpamOperations.Login, user);
            
            return await BuildIndexAsync(userLogin, context);

        }
        
        async Task<User> BuildUserAsync(LoginPage userLogin)
        {

            var user = await _platoUserStore.GetByUserNameAsync(userLogin.UserName);
            if (user == null)
            {
                return null;
            }

            // Ensure we check against the IP address being used at the time of the login
            user.IpV4Address = _clientIpAddress.GetIpV4Address();
            user.IpV6Address = _clientIpAddress.GetIpV6Address();
            return user;
            
        }
    }

}
