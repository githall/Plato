using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Plato.Docs.Models;
using Plato.Entities.Stores;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Security.Abstractions;
using Plato.Entities.Models;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Models.Users;
using Plato.Internal.Notifications.Abstractions;
using Plato.Internal.Notifications.Extensions;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Internal.Stores.Users;
using Plato.Docs.Anchorific.ViewModels;

namespace Plato.Docs.Anchorific.ViewProviders
{

    public class DocViewProvider : BaseViewProvider<Doc>
    {

        private const string FollowHtmlName = "follow";
        private const string NotifyHtmlName = "notify";

        private readonly IUserNotificationTypeDefaults _userNotificationTypeDefaults;
        private readonly IDummyClaimsPrincipalFactory<User> _claimsPrincipalFactory;
        private readonly INotificationManager<Doc> _notificationManager;
        private readonly IAuthorizationService _authorizationService;   
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IEntityStore<Doc> _entityStore;
        private readonly IContextFacade _contextFacade;
        private readonly HttpRequest _request;
 
        public DocViewProvider(
            IUserNotificationTypeDefaults userNotificationTypeDefaults,
            IDummyClaimsPrincipalFactory<User> claimsPrincipalFactory,    
            INotificationManager<Doc> notificationManager,
            IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor,            
            IPlatoUserStore<User> platoUserStore,
            IEntityStore<Doc> entityStore,
            IContextFacade contextFacade)
        {
            _userNotificationTypeDefaults = userNotificationTypeDefaults;
            _request = httpContextAccessor.HttpContext.Request;
            _claimsPrincipalFactory = claimsPrincipalFactory;
            _authorizationService = authorizationService;
            _notificationManager = notificationManager;        
            _platoUserStore = platoUserStore;    
            _contextFacade = contextFacade;      
            _entityStore = entityStore;
        }
        
        public override Task<IViewProviderResult> BuildIndexAsync(Doc entity, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override async Task<IViewProviderResult> BuildDisplayAsync(Doc entity, IViewProviderContext updater)
        {

            if (entity == null)
            {
                return await BuildIndexAsync(new Doc(), updater);
            }

            var viewModel = new AnchorificViewModel();

            return Views(
                View<AnchorificViewModel>("Doc.Anchorific.Asides", model => viewModel).Zone("asides").Order(-4)
            );

        }

        public override Task<IViewProviderResult> BuildEditAsync(Doc entity, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(Doc article, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

    }

}
