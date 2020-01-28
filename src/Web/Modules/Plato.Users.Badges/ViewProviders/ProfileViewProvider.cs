using System.Threading.Tasks;
using PlatoCore.Badges.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Models.Badges;
using PlatoCore.Models.Users;
using PlatoCore.Stores.Abstractions.Badges;
using PlatoCore.Stores.Abstractions.Users;
using Plato.Users.Badges.Services;
using Plato.Users.Badges.ViewModels;

namespace Plato.Users.Badges.ViewProviders
{

    public class ProfileViewProvider : BaseViewProvider<ProfilePage>
    {
        
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IBadgeEntriesStore _badgeEntriesStore;

        public ProfileViewProvider(
            IPlatoUserStore<User> platoUserStore,
            IUserBadgeStore<UserBadge> userBadgeStore,
            IBadgesManager<Badge> badgesManager, IBadgeEntriesStore badgeEntriesStore)
        {
            _platoUserStore = platoUserStore;
            _badgeEntriesStore = badgeEntriesStore;
        }

        public override async Task<IViewProviderResult> BuildDisplayAsync(ProfilePage profile,
            IViewProviderContext context)
        {

            var user = await _platoUserStore.GetByIdAsync(profile.Id);
            if (user == null)
            {
                return await BuildIndexAsync(profile, context);
            }
            
            var badges = await _badgeEntriesStore.SelectByUserIdAsync(user.Id);
            var viewModel = new ProfileDisplayViewModel()
            {
                User = user,
                Badges = badges
            };

            return Views(
                View<ProfileDisplayViewModel>("Profile.Display.Content", model => viewModel).Zone("content").Order(0)                
            );

        }

        public override Task<IViewProviderResult> BuildIndexAsync(ProfilePage model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(ProfilePage model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(ProfilePage model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

    }

}
