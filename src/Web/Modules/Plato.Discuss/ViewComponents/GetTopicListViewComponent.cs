using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Models;
using Plato.Entities.Services;
using Plato.Entities.ViewModels;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Security.Abstractions;
using PlatoCore.Layout.Views.Abstractions;

namespace Plato.Discuss.ViewComponents
{
  
    public class GetTopicListViewComponent : BaseViewComponent
    {
        
      
        private readonly IAuthorizationService _authorizationService;
       
        private readonly IEntityService<Topic> _entityService;

        public GetTopicListViewComponent(
            IEntityService<Topic> entityService,
            IAuthorizationService authorizationService)
        {
            _entityService = entityService;
            _authorizationService = authorizationService;       
        }

        public async Task<IViewComponentResult> InvokeAsync(EntityIndexOptions options, PagerOptions pager)
        {

            // Build default
            if (options == null)
            {
                options = new EntityIndexOptions();
            }

            // Build default
            if (pager == null)
            {
                pager = new PagerOptions();
            }

            var viewModel = await GetViewModel(options, pager);

            // Review view
            return View(viewModel);

        }
        
        async Task<EntityIndexViewModel<Topic>> GetViewModel(
            EntityIndexOptions options,
            PagerOptions pager)
        {

            // Get results
            var results = await _entityService
                .ConfigureQuery(async q =>
                {
                    
                    // Hide private?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewPrivateTopics))
                    {
                        q.HidePrivate.True();
                    }

                    // Hide hidden?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewHiddenTopics))
                    {
                        q.HideHidden.True();
                    }

                    // Hide spam?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewSpamTopics))
                    {
                        q.HideSpam.True();
                    }

                    // Hide deleted?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewDeletedTopics))
                    {
                        q.HideDeleted.True();
                    }



                })
                .GetResultsAsync(options, pager);

            // Set total on pager
            pager.SetTotal(results?.Total ?? 0);
            
            // Return view model
            return new EntityIndexViewModel<Topic>
            {
                Results = results,
                Options = options,
                Pager = pager
            }; 

        }

    }
    
}

