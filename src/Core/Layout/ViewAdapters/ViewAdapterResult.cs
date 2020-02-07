using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;

namespace PlatoCore.Layout.ViewAdapters
{
    
    public class ViewAdapterResult : IViewAdapterResult 
    {

        private IList<Func<IHtmlContent, IHtmlContent>> _outputAlterations;
        private IList<string> _viewAlterations;
        private IList<Func<object, Task<object>>> _modelAlterations;
        
        public IViewAdapterBuilder Builder { get; set; }

        public IList<Func<IHtmlContent, IHtmlContent>> OutputAlterations
        {
            get => _outputAlterations ?? (_outputAlterations = new List<Func<IHtmlContent, IHtmlContent>>());
            set => _outputAlterations = value;
        }

        public IList<string> ViewAlterations
        {
            get => _viewAlterations ?? (_viewAlterations = new List<string>());
            set => _viewAlterations = value;
        }

        public IList<Func<object, Task<object>>> ModelAlterations
        {
            get => _modelAlterations ?? (_modelAlterations = new List<Func<object, Task<object>>>());
            set => _modelAlterations = value;
        }
   
    }

}
