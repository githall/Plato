namespace PlatoCore.Models.Tour
{
    public static class DefaultSteps
    {

        public static TourStep EnablleCoreFeature = new TourStep()
        {
            Id = "Tour.Features.Core", /* should not change */
            Title = "Enable Core Features",
            Description = "Customize Plato by enabling one or more core features.",
            ViewName = ""
        };

        public static TourStep EnablleOptionalFeature = new TourStep()
        {
            Id = "Tour.Features.Optional",/* should not change */
            Title = "Enable Optional Features",
            Description = "Customize Plato further by enabling one or more optional features.",
            ViewName = ""
        };

        public static TourStep UpdateProfile = new TourStep()
        {
            Id = "Tour.Update.Profile", /* should not change */
            Title = "Update Your Profile",
            Description = "Check your happy with your user profile."
        };

        public static TourStep GeneralSettings = new TourStep()
        {
            Id = "Tour.General.Settings", /* should not change */
            Title = "Update General Settings",
            Description = "Customize general application settings."
        };

        public static TourStep EmailSettings = new TourStep()
        {
            Id = "Tour.Email.Settings", /* should not change */
            Title = "Update Email Settings",
            Description = "Customize outbound email settings."
        };

        public static TourStep EnableSearch = new TourStep()
        {
            Id = "Tour.Features.Search", /* should not change */
            Title = "Enable Search",
            Description = "Instantly search across all your support content."
        };

    }

}
