using PlatoCore.Models.Tour;
using System.Collections.Generic;

namespace Plato.Tour.Models
{
    public static class TourDescriptorSteps
    {
        public static TourStep EnablleCoreFeature = new TourStep()
        {
            Id = "Tour.Features.Core",
            Title = "Enable Core Features",
            Description = "Customize Plato by enabling one or more core features.",
            ViewName = ""
        };

        public static TourStep EnablleOptionalFeature = new TourStep()
        {
            Id = "Tour.Features.Optional",
            Title = "Enable Optional Features",
            Description = "Customize Plato further by enabling one or more optional features.",
            ViewName = ""
        };

        public static TourStep UpdateProfile = new TourStep()
        {
            Id = "Tour.Update.Profile",
            Title = "Update Profile",
            Description = "Check your happy your profile is correct."
        };

        public static TourStep GeneralSettings = new TourStep()
        {
            Id = "Tour.General.Settings",
            Title = "General Settings",
            Description = "Customize general application settings."
        };

        public static TourStep EmailSettings = new TourStep()
        {
            Id = "Tour.Email.Settings",
            Title = "Email Settings",
            Description = "Customize general application settings."
        };

        public static TourStep EnableSearch = new TourStep()
        {
            Id = "Tour.Features.Search",
            Title = "Enable Search",
            Description = "Enable Search"
        };

        public static IEnumerable<TourStep> Steps = new List<TourStep>()
        {
            EnablleCoreFeature,
            EnablleOptionalFeature,
            UpdateProfile,
            GeneralSettings,
            EmailSettings,
            EnableSearch
        };

    }

}
