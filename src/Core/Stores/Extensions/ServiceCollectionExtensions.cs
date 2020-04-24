using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Data.Abstractions;
using PlatoCore.Models.Badges;
using PlatoCore.Models.Features;
using PlatoCore.Models.Reputations;
using PlatoCore.Models.Users;
using PlatoCore.Stores.Abstract;
using PlatoCore.Stores.Abstractions.Badges;
using PlatoCore.Stores.Abstractions.Files;
using PlatoCore.Stores.Abstractions.Reputations;
using PlatoCore.Stores.Abstractions.Roles;
using PlatoCore.Stores.Abstractions.Schema;
using PlatoCore.Stores.Abstractions.Settings;
using PlatoCore.Stores.Abstractions.Shell;
using PlatoCore.Stores.Abstractions.Users;
using PlatoCore.Stores.Badges;
using PlatoCore.Stores.Files;
using PlatoCore.Stores.Reputations;
using PlatoCore.Stores.Roles;
using PlatoCore.Stores.Schema;
using PlatoCore.Stores.Settings;
using PlatoCore.Stores.Shell;
using PlatoCore.Stores.Users;

namespace PlatoCore.Stores.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddShellStores(
            this IServiceCollection services)
        {

            // Abstract stores 
            services.AddScoped<IDictionaryStore, DictionaryStore>();
            services.AddScoped<IDocumentStore, DocumentStore>();

            // Ensure query is aware of current db context
            services.AddScoped<IDbQueryConfiguration, DbQueryConfiguration>();

            // Files
            services.AddSingleton<IFileStore, FileStore>();

            // Shell features
            services.AddScoped<IShellDescriptorStore, ShellDescriptorStore>();
            services.AddScoped<IShellFeatureStore<ShellFeature>, ShellFeatureStore>();

            // Site Settings
            services.AddScoped<ISiteSettingsStore, SiteSettingsStore>();

            // Roles
            services.AddScoped<IPlatoRoleStore, PlatoRoleStore>();
            services.AddScoped<IPlatoUserRoleStore<UserRole>, PlatoUserRolesStore>();

            // Users
            services.AddScoped<IPlatoUserStore<User>, PlatoUserStore>();
            services.AddScoped<IPlatoUserLoginStore<UserLogin>, PlatoUserLoginStore>();
            services.AddScoped<IUserPhotoStore<UserPhoto>, UserPhotoStore>();            
            services.AddScoped<IUserDataItemStore<UserData>, UserDataItemStore>();
            services.AddScoped<IUserDataStore<UserData>, UserDataStore>();

            // Decorators
            services.AddScoped<IUserDataDecorator, UserDataDecorator>();
            services.AddScoped<IUserRoleDecorator, UserRoleDecorator>();

            // User Reputation
            services.AddScoped<IUserReputationsStore<UserReputation>, UserReputationsStore>();

            // User badges
            services.AddScoped<IUserBadgeStore<UserBadge>, UserBadgeStore>();

            // Schema
            services.AddScoped<IConstraintStore, ConstraintStore>();

            return services;

        }

    }

}
