using System;
using Microsoft.AspNetCore.Mvc;
using PlatoCore.Layout.ModelBinding;

namespace PlatoCore.Layout.ViewProviders.Abstractions
{

    public interface IViewProviderContext
    {

        IUpdateModel Updater { get; set; }

        Controller Controller { get; }
        
    }

    public class ViewProviderContext : IViewProviderContext
    {

        public IUpdateModel Updater { get; set; }

        public Controller Controller { get; }
        
        public ViewProviderContext(IUpdateModel updater)
        {
            Updater = updater ?? throw new ArgumentNullException(nameof(updater));
            Controller = updater as Controller ?? throw new Exception($"Could not convert type of '{GetType()}' to type of 'Controller'.");
        }
    }
}
