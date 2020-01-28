using System.Data;
using PlatoCore.Models.Annotations;

namespace PlatoCore.Models.Users
{
    [TableName("Plato_UserBanner")]
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