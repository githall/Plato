﻿using System;
using System.Threading.Tasks;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Models.Users;
using Plato.StopForumSpam.Services;
using Plato.Users.StopForumSpam.ViewModels;

namespace Plato.Users.StopForumSpam.ViewProviders
{
    public class AdminViewProvider : ViewProviderBase<User>
    {

    
        private readonly ISpamChecker _spamChecker;
        
        public AdminViewProvider(
            ISpamChecker spamChecker)
        {
            _spamChecker = spamChecker;
        }

        #region "Implementation"

        public override Task<IViewProviderResult> BuildIndexAsync(User user, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildDisplayAsync(User user, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(User user, IViewProviderContext updater)
        {

            // No need to perform spam checking when creating new users
            if (user.Id == 0)
            {
                return default(IViewProviderResult);
            }

            // Build edit view model
            var viewModel = new StopForumSpamViewModel()
            {
                Id = user.Id,
                IsNewUser = user.Id == 0,
                IsSpam = user.IsSpam,
                IsVerified = user.IsVerified,
                IsStaff = user.IsStaff,
                IsBanned = user.IsBanned,
                Checker = await _spamChecker.CheckAsync(user)
            };

            // Build view
            return Views(
                View<StopForumSpamViewModel>("Admin.Edit.StopForumSpam.Sidebar", model => viewModel).Zone("content-right")
                    .Order(int.MinValue)
            );
            
        }
        
        public override async Task<IViewProviderResult> BuildUpdateAsync(User user, IViewProviderContext context)
        {
            return await BuildEditAsync(user, context);
        }

        #endregion
        
    }

}
