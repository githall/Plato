using System.Data;
using PlatoCore.Models.Annotations;

namespace PlatoCore.Models.Users
{

    public class UserPhoto : UserImage
    {
        public UserPhoto()
        {
        }

        public UserPhoto(IDataReader reader)
            : base(reader)
        {
        }
    }
}