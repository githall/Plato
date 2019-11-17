using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Internal.Stores.Users
{

    public class UserLoginDecorator : IUserLoginDecorator
    {

        private readonly IPlatoUserLoginStore<UserLogin> _platoUserLoginStore;        

        public UserLoginDecorator(IPlatoUserLoginStore<UserLogin> platoUserLoginStore)
        {
            _platoUserLoginStore = platoUserLoginStore;
        }

        public async Task<IEnumerable<User>> DecorateAsync(IEnumerable<User> users)
        {

            if (users == null)
            {
                return null;
            }

            var logins = await _platoUserLoginStore.QueryAsync()
                .Select<UserDataQueryParams>(q =>
                {
                    q.UserId.IsIn(users.Select(u => u.Id).ToArray());
                })
                .ToList();

            if (logins == null)
            {
                return users;
            }

            // Merge data into users
            return MergeData(users.ToList(), logins.Data);

        }

        public async Task<User> DecorateAsync(User user)
        {

            if (user == null)
            {
                return null;
            }
                        
            var logins = await _platoUserLoginStore.QueryAsync()
                .Select<UserDataQueryParams>(q =>
                {
                    q.UserId.Equals(user.Id);
                })
                .ToList();

            if (logins?.Data != null)
            {
                user.LoginInfos.Clear();
                foreach (var login in logins.Data)
                {
                    user.LoginInfos.Add(login);
                }
            }

            return user;

        }

        // -----------

        IList<User> MergeData(IList<User> users, IList<UserLogin> data)
        {

            if (users == null || data == null)
            {
                return users;
            }

            for (var i = 0; i < users.Count; i++)
            {
                users[i].LoginInfos = data.Where(d => d.UserId == users[i].Id).ToList();                
            }

            return users;

        }

    }

}
