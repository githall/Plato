﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PlatoCore.Abstractions.SetUp;
using PlatoCore.Data.Migrations.Abstractions;
using PlatoCore.Layout.ActionFilters;
using PlatoCore.Models.Roles;
using PlatoCore.Models.Users;
using PlatoCore.Stores.Users;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Layout.ViewProviders;
using PlatoCore.Messaging.Abstractions;
using PlatoCore.Models.Reputations;
using PlatoCore.Models.Shell;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Reputations.Abstractions;
using PlatoCore.Security;
using Plato.Users.Handlers;
using Plato.Users.ViewModels;
using Plato.Users.ViewProviders;
using PlatoCore.Security.Abstractions;
using Plato.Users.ActionFilters;
using Plato.Users.Middleware;
using Plato.Users.Navigation;
using Plato.Users.Services;
using Plato.Users.Subscribers;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Hosting.Abstractions;

namespace Plato.Users
{
    public class Startup : StartupBase
    {

        private readonly string _cookieSuffix;
        private readonly string _cookiePath;

        public Startup(IShellSettings shellSettings)
        {
            _cookieSuffix = shellSettings.AuthCookieName;
            _cookiePath = !string.IsNullOrEmpty(shellSettings.RequestedUrlPrefix)
                ? $"/{shellSettings.RequestedUrlPrefix}"
                : "/";
        }

        public override void ConfigureServices(IServiceCollection services)
        {

            // Register set-up event handler
            services.AddScoped<ISetUpEventHandler, SetUpEventHandler>();

            // --------

            // Adds the default token providers used to generate tokens for reset passwords, change email
            // and change telephone number operations, and for two factor authentication token generation.
            services.AddIdentity<User, Role>().AddDefaultTokenProviders();

            // Add authentication services & configure default authentication scheme
            services.AddAuthentication(options =>
            {                
                options.DefaultSignOutScheme = IdentityConstants.ApplicationScheme;
            });

            // --------

            // Replace the default UserClaimsPrincipalFactory implementation added via AddIdentity()
            // above with our own PlatoClaimsPrincipalFactory implementation, this ensures role claims
            // are not stored within the client cookie
            services.Replace<IUserClaimsPrincipalFactory<User>, PlatoUserClaimsPrincipalFactory<User, Role>>(ServiceLifetime.Scoped);

            // Build a claim principal from the given user
            services.TryAddScoped<IDummyClaimsPrincipalFactory<User>, DummyClaimsPrincipalFactory<User>>();

            // Stores
            services.TryAddScoped<IUserStore<User>, UserStore>();
            services.TryAddScoped<IUserSecurityStampStore<User>, UserStore>();

            // User color provider 
            services.AddSingleton<IUserColorProvider, UserColorProvider>();

            // Custom User Manager
            services.AddScoped<IPlatoUserManager<User>, PlatoUserManager<User>>();

            // User account emails
            services.TryAddScoped<IUserEmails, UserEmails>();

            // Configure authentication cookie options
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = $"plato_{ _cookieSuffix.ToLower()}";
                options.Cookie.Path = _cookiePath;
                options.LoginPath = new PathString(StatusCodePagePaths.Login);
                options.AccessDeniedPath = new PathString(StatusCodePagePaths.Unauthorized);                
                options.ExpireTimeSpan = TimeSpan.FromDays(30);
                options.SlidingExpiration = true;
            });

            services.ConfigureExternalCookie(options =>
            {
                options.Cookie.Name = $"plato_external_{ _cookieSuffix.ToLower()}";
                options.Cookie.Path = _cookiePath;
                options.LoginPath = new PathString(StatusCodePagePaths.Login);
                options.AccessDeniedPath = new PathString(StatusCodePagePaths.Unauthorized);
                options.ExpireTimeSpan = TimeSpan.FromDays(30);
                options.SlidingExpiration = true;
            });

            // Configure IdentityOptions
            services.Configure<IdentityOptions>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;                
            });

            // Navigation providers
            services.AddScoped<INavigationProvider, AdminMenu>();
            services.AddScoped<INavigationProvider, AdminUserMenu>();
            services.AddScoped<INavigationProvider, SiteMenu>();
            services.AddScoped<INavigationProvider, UserMenu>();
            services.AddScoped<INavigationProvider, EditProfileMenu>();
            services.AddScoped<INavigationProvider, ProfileMenu>();

            // Admin view providers
            services.AddScoped<IViewProviderManager<User>, ViewProviderManager<User>>();
            services.AddScoped<IViewProvider<User>, AdminViewProvider>();

            // Profile view providers
            services.AddScoped<IViewProviderManager<ProfilePage>, ViewProviderManager<ProfilePage>>();
            services.AddScoped<IViewProvider<ProfilePage>, UserViewProvider>();

            // Edit profile view provider
            services.AddScoped<IViewProviderManager<EditProfileViewModel>, ViewProviderManager<EditProfileViewModel>>();
            services.AddScoped<IViewProvider<EditProfileViewModel>, EditProfileViewProvider>();

            // Edit account view provider
            services.AddScoped<IViewProviderManager<EditAccountViewModel>, ViewProviderManager<EditAccountViewModel>>();
            services.AddScoped<IViewProvider<EditAccountViewModel>, EditAccountViewProvider>();

            // Edit user settings view provider
            services.AddScoped<IViewProviderManager<EditSettingsViewModel>, ViewProviderManager<EditSettingsViewModel>>();
            services.AddScoped<IViewProvider<EditSettingsViewModel>, EditSettingsViewProvider>();

            // Edit user signature view provider
            services.AddScoped<IViewProviderManager<EditSignatureViewModel>, ViewProviderManager<EditSignatureViewModel>>();
            services.AddScoped<IViewProvider<EditSignatureViewModel>, EditSignatureViewProvider>();

            // Login view provider
            services.AddScoped<IViewProviderManager<LoginPage>, ViewProviderManager<LoginPage>>();
            services.AddScoped<IViewProvider<LoginPage>, LoginViewProvider>();

            // Register view provider
            services.AddScoped<IViewProviderManager<UserRegistration>, ViewProviderManager<UserRegistration>>();
            services.AddScoped<IViewProvider<UserRegistration>, RegisterViewProvider>();

            // Register permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

            // Register reputation providers
            services.AddScoped<IReputationsProvider<Reputation>, Reputations>();

            // Register user service
            services.AddScoped<IUserService<User>, UserService<User>>();

            // Register action filters
            services.AddScoped<IModularActionFilter, InvalidateUserFilter>();
            services.AddScoped<IModularActionFilter, UpdateUserLastLoginDateFilter>();

            // Migrations
            services.AddSingleton<IMigrationProvider, Migrations>();

            // Broker subscriptions
            services.AddScoped<IBrokerSubscriber, ParseSignatureHtmlSubscriber>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            // Register tenant authentication middle ware
            app.UseAuthentication();

            // Register authenticated user middle ware
            // Must be registered after .NET authentication middle ware has been 
            // registered i.e. after app.UseAuthentication() above
            app.UseMiddleware<AuthenticatedUserMiddleware>();

            // --------------
            // Account
            // --------------

            routes.MapAreaRoute(
                name: "Login",
                areaName: "Plato.Users",
                template: "login",
                defaults: new { controller = "Account", action = "Login" }
            );

            routes.MapAreaRoute(
                name: "Logout",
                areaName: "Plato.Users",
                template: "logout",
                defaults: new { controller = "Account", action = "Logoff" }
            );

            routes.MapAreaRoute(
                name: "Register",
                areaName: "Plato.Users",
                template: "register",
                defaults: new { controller = "Account", action = "Register" }
            );

            routes.MapAreaRoute(
                name: "ConfirmEmail",
                areaName: "Plato.Users",
                template: "account/confirm",
                defaults: new { controller = "Account", action = "ConfirmEmail" }
            );

            routes.MapAreaRoute(
                name: "ConfirmEmailConfirmation",
                areaName: "Plato.Users",
                template: "account/confirm/success",
                defaults: new { controller = "Account", action = "ConfirmEmailConfirmation" }
            );

            routes.MapAreaRoute(
                name: "ActivateAccount",
                areaName: "Plato.Users",
                template: "account/activate",
                defaults: new { controller = "Account", action = "ActivateAccount" }
            );

            routes.MapAreaRoute(
                name: "ActivateAccountConfirmation",
                areaName: "Plato.Users",
                template: "account/activate/success",
                defaults: new { controller = "Account", action = "ActivateAccountConfirmation" }
            );

            routes.MapAreaRoute(
                name: "ForgotPassword",
                areaName: "Plato.Users",
                template: "account/forgot",
                defaults: new { controller = "Account", action = "ForgotPassword" }
            );

            routes.MapAreaRoute(
                name: "ForgotPasswordConfirmation",
                areaName: "Plato.Users",
                template: "account/forgot/success",
                defaults: new { controller = "Account", action = "ForgotPasswordConfirmation" }
            );

            routes.MapAreaRoute(
                name: "ResetPassword",
                areaName: "Plato.Users",
                template: "account/reset",
                defaults: new { controller = "Account", action = "ResetPassword" }
            );

            routes.MapAreaRoute(
                name: "ResetPasswordConfirmation",
                areaName: "Plato.Users",
                template: "account/reset/success",
                defaults: new { controller = "Account", action = "ResetPasswordConfirmation" }
            );

            routes.MapAreaRoute(
                name: "ExternalLogin",
                areaName: "Plato.Users",
                template: "external-login",
                defaults: new { controller = "Account", action = "ExternalLogin" }
            );

            routes.MapAreaRoute(
                name: "ExternalLoginCallback",
                areaName: "Plato.Users",
                template: "external-login-callback",
                defaults: new { controller = "Account", action = "ExternalLoginCallback" }
            );

            routes.MapAreaRoute(
                name: "ExternalLogins",
                areaName: "Plato.Users",
                template: "external-logins",
                defaults: new { controller = "Account", action = "ExternalLogins" }
            );

            // --------------
            // Users
            // --------------

            routes.MapAreaRoute(
                name: "UsersIndex",
                areaName: "Plato.Users",
                template: "u/{pager.offset:int?}",
                defaults: new { controller = "Home", action = "Index" }
            );

            routes.MapAreaRoute(
                name: "DisplayUser",
                areaName: "Plato.Users",
                template: "u/{opts.id:int}/{opts.alias?}",
                defaults: new { controller = "Home", action = "Display" }
            );

            routes.MapAreaRoute(
                name: "GetUser",
                areaName: "Plato.Users",
                template: "u/get/{opts.id:int}/{opts.alias?}",
                defaults: new { controller = "Home", action = "GetUser" }
            );

            routes.MapAreaRoute(
                name: "UserLetter",
                areaName: "Plato.Users",
                template: "u/l/{letter}/{color}",
                defaults: new { controller = "Letter", action = "Get" }
            );

            // --------------
            // Profile
            // --------------

            routes.MapAreaRoute(
                name: "EditUserProfile",
                areaName: "Plato.Users",
                template: "profile",
                defaults: new { controller = "Home", action = "EditProfile" }
            );

            routes.MapAreaRoute(
                name: "EditUserAccount",
                areaName: "Plato.Users",
                template: "profile/account",
                defaults: new { controller = "Home", action = "EditAccount" }
            );

            routes.MapAreaRoute(
                name: "EditUserSignature",
                areaName: "Plato.Users",
                template: "profile/signature",
                defaults: new { controller = "Home", action = "EditSignature" }
            );

            routes.MapAreaRoute(
                name: "EditUserSettings",
                areaName: "Plato.Users",
                template: "profile/settings",
                defaults: new { controller = "Home", action = "EditSettings" }
            );

            // --------------
            // Admin Routes
            // --------------

            routes.MapAreaRoute(
                name: "AdminUsersOffset",
                areaName: "Plato.Users",
                template: "admin/users/{pager.offset:int?}",
                defaults: new { controller = "Admin", action = "Index" }
            );

            routes.MapAreaRoute(
                name: "Admin-Users",
                areaName: "Plato.Users",
                template: "admin/users/{action}/{id?}",
                defaults: new { controller = "Admin", action = "Index" }
            );

        }

    }

}