using System.Collections.Generic;
using Plato.Follows.Models;
using Plato.Follows.Services;

namespace Plato.Issues.Labels.Follow
{

    public class FollowTypes : IFollowTypeProvider
    {

        public static readonly FollowType Label =
            new FollowType(
                "IssueLabel",
                "Follow Label",
                "Follow this label to get notified when new issues are posted with this label",
                "Unsubscribe",
                "You are following this label and will be notified when issues are posted with this label, click to unsubscribe",
                "Login to follow this label",
                "You don't have permission to follow this label");

        public IEnumerable<IFollowType> GetFollowTypes()
        {
            return new[]
            {
                Label
            };
        }

    }

}
