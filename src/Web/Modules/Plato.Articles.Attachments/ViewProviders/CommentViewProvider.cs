using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Plato.Articles.Models;
using Plato.Entities.Stores;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Security.Abstractions;

namespace Plato.Articles.Attachments.ViewProviders
{

    public class CommentViewProvider : ViewProviderBase<Comment>
    {
   
        private readonly IAuthorizationService _authorizationService;
        private readonly IEntityStore<Article> _entityStore;
        private readonly IContextFacade _contextFacade;

        public CommentViewProvider(
            IAuthorizationService authorizationService,
            IEntityStore<Article> entityStore,
            IContextFacade contextFacade)
        {
            _authorizationService = authorizationService;       
            _contextFacade = contextFacade;
            _entityStore = entityStore;      
        }
        
        public override Task<IViewProviderResult> BuildDisplayAsync(Comment model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildIndexAsync(Comment model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(Comment reply, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(Comment reply, IViewProviderContext context)
        {

            if (reply == null)
            {
                return await BuildIndexAsync(new Comment(), context);
            }
            
            return await BuildEditAsync(reply, context);

        }

    }

}
