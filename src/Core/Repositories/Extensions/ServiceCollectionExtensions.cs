using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Models;
using PlatoCore.Models.Abstract;
using PlatoCore.Models.Badges;
using PlatoCore.Models.Features;
using PlatoCore.Models.Reputations;
using PlatoCore.Models.Roles;
using PlatoCore.Models.Users;
using PlatoCore.Repositories.Abstract;
using PlatoCore.Repositories.Badges;
using PlatoCore.Repositories.Metrics;
using PlatoCore.Repositories.Reputations;
using PlatoCore.Repositories.Roles;
using PlatoCore.Repositories.Schema;
using PlatoCore.Repositories.Shell;
using PlatoCore.Repositories.Users;

namespace PlatoCore.Repositories.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoRepositories(
            this IServiceCollection services)
        {

            // Shell features
            services.AddScoped<IShellFeatureRepository<ShellFeature>, ShellFeatureRepository>();
            
            // Abstract storage (used for unique key value paris - i.e. global settings
            services.AddScoped<IDictionaryRepository<DictionaryEntry>, DictionaryRepository>();
            services.AddScoped<IDocumentRepository, DocumentRepository>();

            // Roles
            services.AddScoped<IRoleRepository<Role>, RoleRepository>();

            // Users
            services.AddScoped<IUserRepository<User>, UserRepository>();
            services.AddScoped<IUserSecretRepository<UserSecret>, UserSecretRepository>();
            services.AddScoped<IUserDataRepository<UserData>, UserDataRepository>();
            services.AddScoped<IUserLoginRepository<UserLogin>, UserLoginRepository>();
            services.AddScoped<IUserPhotoRepository<UserPhoto>, UserPhotoRepository>();
            services.AddScoped<IUserBannerRepository<UserBanner>, UserBannerRepository>();
            services.AddScoped<IUserRolesRepository<UserRole>, UserRolesRepository>();
            services.AddScoped<IRoleRepository<Role>, RoleRepository>();

            // User reputations
            services.AddScoped<IUserReputationsRepository<UserReputation>, UserReputationsRepository>();
            services.AddScoped<IAggregatedUserReputationRepository, AggregatedUserReputationRepository>();

            // User badges
            services.AddScoped<IUserBadgeRepository<UserBadge>, UserBadgeRepository>();

            // Schema 
            services.AddScoped<IConstraintRepository, ConstraintRepository>();

            // Metrics 
            services.AddScoped<IAggregatedUserRepository, AggregatedUserRepository>();
            services.AddScoped<IBadgeDetailsRepository, BadgeDetailsRepository>();

            return services;

        }

    }

}
