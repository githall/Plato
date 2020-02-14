using System.Data;

namespace PlatoCore.Models.Users
{
 
    public class UserBanner : UserImage
    {
        public UserBanner()
        {
        }

        public UserBanner(IDataReader reader)
            : base(reader)
        {
        }
    }
}