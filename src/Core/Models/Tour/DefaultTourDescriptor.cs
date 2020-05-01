using System.Collections.Generic;

namespace PlatoCore.Models.Tour
{
    public class DefaultTourDescriptor : TourDescriptor
    {
        public DefaultTourDescriptor()
        {
            Steps = new List<TourStep>()
            {
                DefaultSteps.EnablleCoreFeature,
                DefaultSteps.EnablleOptionalFeature,
                DefaultSteps.UpdateProfile,
                DefaultSteps.GeneralSettings,
                DefaultSteps.EmailSettings,
                DefaultSteps.EnableSearch
            };
        }

    }

}
