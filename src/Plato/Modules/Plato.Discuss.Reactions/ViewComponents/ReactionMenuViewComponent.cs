﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Models;
using Plato.Discuss.Reactions.ViewModels;
using Plato.Entities.Stores;
using Plato.Reactions.Models;
using Plato.Reactions.Services;

namespace Plato.Discuss.Reactions.ViewComponents
{
  
    public class ReactionMenuViewComponent : ViewComponent
    {
  
        private readonly IReactionsManager<Reaction> _reactionManager;

        public ReactionMenuViewComponent(
            IReactionsManager<Reaction> reactionManager)
        {
      
            _reactionManager = reactionManager;
        }

        public Task<IViewComponentResult> InvokeAsync(
            Topic topic,
            Reply reply)
        {
    
            var viewModel = new ReactionMenuViewModel()
            {
                Topic = topic,
                Reply = reply,
                Reactions = _reactionManager.GetReactions()
            };

            return Task.FromResult((IViewComponentResult) View(viewModel));
        }

    }

}