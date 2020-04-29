using System;

namespace PlatoCore.Models.Tour
{
  
    public class TourStep
    {

        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string ViewName { get; set; }

        public DateTimeOffset? CompletedDate { get; set; }

    }
}
