using System.Collections.Generic;

namespace PlatoCore.Models.Tour
{
    public static class DefaultTourDescriptorSteps
    {

        public static IEnumerable<TourStep> Steps = new List<TourStep>()
        {
            new TourStep()
            {
                Id = "Features",
                Title = "Enable Features",
                Description = "Customize Plato by enabling features."
            },
            new TourStep()
            {
                Id = "Profile",
                Title = "Update Profile",
                Description = "Check your happy with your profile."
            },
            new TourStep()
            {
                Id = "GeneralSettings",
                Title = "General Settings",
                Description = "Customize general application settings."
            },
            new TourStep()
            {
                Id = "EmailSettings",
                Title = "Email Settings",
                Description = "Customize general application settings.",
                CompletedDate = System.DateTimeOffset.Now
            }
        };

    }

}
