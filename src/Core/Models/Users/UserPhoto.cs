using System.Data;

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