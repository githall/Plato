using PlatoCore.Models.Tour;
using System.Collections.Generic;

namespace Plato.Tour.ViewModels
{
    public class TourIndexViewModel
    {
        public IEnumerable<TourStep> Steps { get; set; }

        public int TotalSteps
        {
            get
            {
                var i = 0;
                foreach (var step in Steps)
                {
                    i++;
                }
                return i;
            }
        }

        public int CompletedSteps
        {
            get
            {
                var i = 0;
                foreach (var step in Steps)
                {
                    if (step.CompletedDate.HasValue)
                    {
                        i++;
                    }                    
                }
                return i;
            }
        }

    }

}
