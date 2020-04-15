using System;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using PlatoCore.Models.Roles;
using PlatoCore.Models.Users;
using PlatoCore.Abstractions.SetUp;
using PlatoCore.Security.Abstractions;
using PlatoCore.Data.Schemas.Abstractions;
using Plato.Users.Services;

namespace Plato.Users.Handlers
{


    public class SetUpEventHandler : BaseSetUpEventHandler
    {

        private const string Version = "1.1.0";
        
        private readonly SchemaTable _users = UserTables.Users;
        private readonly SchemaTable _userBanner = UserTables.UserBanner;
        private readonly SchemaTable _userPhoto = UserTables.UserPhoto;
        private readonly SchemaTable _userData = UserTables.UserData;
        private readonly SchemaTable _userLogin = UserTables.UserLogin;

        private readonly IUserColorProvider _userColorProvider;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly ISchemaBuilder _schemaBuilder;
        private readonly ISchemaManager _schemaManager;

        public SetUpEventHandler(
            IUserColorProvider userColorProvider,            
            UserManager<User> userManager,            
            RoleManager<Role> roleManager,
            ISchemaManager schemaManager,
            ISchemaBuilder schemaBuilder)
        {
            _userColorProvider = userColorProvider;
            _schemaBuilder = schemaBuilder;
            _schemaManager = schemaManager;
            _userManager = userManager;            
            _roleManager = roleManager;
        }

        #region "Implementation"

        public override async Task SetUp(ISetUpContext context, Action<string, string> reportError)
        {

            // Build schemas
            using (var builder = _schemaBuilder)
            {

                // configure
                Configure(builder);

                // user schema
                Users(builder);

                // photo schema
                UserPhoto(builder);

                // banner schema
                UserBanner(builder);

                // user logins schema
                UserLogin(builder);

                // meta data schema
                UserData(builder);
                
                var errors = await _schemaManager.ExecuteAsync(builder.Statements);
                foreach (var error in errors)
                {
                    reportError(error, $"SetUp within {this.GetType().FullName} - {error}");
                }
                

            }

            // Configure default users
            await ConfigureDefaultUsers(context, reportError);

            // Configure administrator
            await ConfigureSuperUser(context, reportError);

        }

        #endregion

        #region "Private Methods"

        void Configure(ISchemaBuilder builder)
        {

            builder
                .Configure(options =>
                {
                    options.ModuleName = base.ModuleId;
                    options.Version = Version;
                    options.DropTablesBeforeCreate = true;
                    options.DropProceduresBeforeCreate = true;
                });

        }

        void Users(ISchemaBuilder builder)
        {

            builder.TableBuilder.CreateTable(_users);
            builder.ProcedureBuilder.CreateDefaultProcedures(_users)

                // Overwrite our SelectEntityById created via CreateDefaultProcedures
                // above to also return all EntityData within a second result set
                .CreateProcedure(
                    new SchemaProcedure(
                            $"SelectUserById",
                            @"
                                /* user */
                                SELECT * FROM {prefix}_Users WITH (nolock) 
                                WHERE (
                                   Id = @Id
                                )
                                /* user data */
                                SELECT * FROM {prefix}_UserData WITH (nolock) 
                                WHERE (
                                   UserId = @Id
                                )
                                /* roles */
                                SELECT r.* FROM {prefix}_UserRoles ur WITH (nolock) 
                                INNER JOIN {prefix}_Roles r ON ur.RoleId = r.Id 
                                WHERE (
                                    ur.UserId = @Id
                                )
                                /* logins */
                                SELECT l.* FROM {prefix}_UserLogins l WITH (nolock)                                 
                                WHERE (
                                    l.UserId = @Id
                                )")
                        .ForTable(_users)
                        .WithParameter(_users.PrimaryKeyColumn))

                .CreateProcedure(new SchemaProcedure("SelectUserByEmail", @"
                            DECLARE @Id int;
                            SET @Id = (SELECT Id FROM {prefix}_Users WITH (nolock) 
                                WHERE (
                                   Email = @Email
                            ))
                            EXEC {prefix}_SelectUserById @id;")
                    .ForTable(_users)
                    .WithParameter(new SchemaColumn() {Name = "Email", DbType = DbType.String, Length = "255"}))


                .CreateProcedure(new SchemaProcedure("SelectUserByEmailNormalized", @"
                            DECLARE @Id int;
                            SET @Id = (SELECT Id FROM {prefix}_Users WITH (nolock) 
                                WHERE (
                                   NormalizedEmail = @NormalizedEmail
                            ))
                            EXEC {prefix}_SelectUserById @id;")
                    .ForTable(_users)
                    .WithParameter(
                        new SchemaColumn() {Name = "NormalizedEmail", DbType = DbType.String, Length = "255"}))
                        
                .CreateProcedure(new SchemaProcedure("SelectUserByUserName", @"
                            DECLARE @Id int;
                            SET @Id = (SELECT Id FROM {prefix}_Users WITH (nolock) 
                                WHERE (
                                   UserName = @UserName
                            ))
                            EXEC {prefix}_SelectUserById @id;")
                    .ForTable(_users)
                    .WithParameter(new SchemaColumn() {Name = "UserName", DbType = DbType.String, Length = "255"}))

                .CreateProcedure(
                    new SchemaProcedure("SelectUserByUserNameNormalized", @"
                            DECLARE @Id int;
                            SET @Id = (SELECT Id FROM {prefix}_Users WITH (nolock) 
                                WHERE (
                                   NormalizedUserName = @NormalizedUserName
                            ))
                            EXEC {prefix}_SelectUserById @id;")
                        .ForTable(_users)
                        .WithParameter(new SchemaColumn()
                        {
                            Name = "NormalizedUserName",
                            DbType = DbType.String,
                            Length = "255"
                        }))
                        
                .CreateProcedure(new SchemaProcedure("SelectUserByApiKey", @"
                            DECLARE @Id int;
                            SET @Id = (SELECT Id FROM {prefix}_Users WITH (nolock) 
                                WHERE (
                                   ApiKey = @ApiKey
                            ))
                            EXEC {prefix}_SelectUserById @id;")
                    .ForTable(_users)
                    .WithParameter(new SchemaColumn() {Name = "ApiKey", DbType = DbType.String, Length = "255"}))

                .CreateProcedure(
                    new SchemaProcedure("SelectUserByEmailAndPassword", @"
                            DECLARE @Id int;
                            SET @Id = (SELECT Id FROM {prefix}_Users WITH (nolock) 
                                WHERE (
                                   Email = @Email AND PasswordHash = @PasswordHash
                            ))
                            EXEC {prefix}_SelectUserById @id;")
                        .ForTable(_users)
                        .WithParameters(new List<SchemaColumn>()
                        {
                            new SchemaColumn()
                            {
                                Name = "Email",
                                DbType = DbType.String,
                                Length = "255"
                            },
                            new SchemaColumn()
                            {
                                Name = "PasswordHash",
                                DbType = DbType.String,
                                Length = "255"
                            }
                        }))

                .CreateProcedure(
                    new SchemaProcedure("SelectUserByResetToken", @"
                            DECLARE @Id int;
                            SET @Id = (SELECT Id FROM {prefix}_Users WITH (nolock) 
                                WHERE (
                                   ResetToken = @ResetToken
                            ))
                            EXEC {prefix}_SelectUserById @id;")
                        .ForTable(_users)
                        .WithParameters(new List<SchemaColumn>()
                        {
                            new SchemaColumn()
                            {
                                Name = "ResetToken",
                                DbType = DbType.String,
                                Length = "255"
                            }
                        }))

                .CreateProcedure(
                    new SchemaProcedure("SelectUserByConfirmationToken", @"
                            DECLARE @Id int;
                            SET @Id = (SELECT Id FROM {prefix}_Users WITH (nolock) 
                                WHERE (
                                   ConfirmationToken = @ConfirmationToken
                            ))
                            EXEC {prefix}_SelectUserById @id;")
                        .ForTable(_users)
                        .WithParameters(new List<SchemaColumn>()
                        {
                            new SchemaColumn()
                            {
                                Name = "ConfirmationToken",
                                DbType = DbType.String,
                                Length = "255"
                            }
                        }))

                .CreateProcedure(new SchemaProcedure("SelectUsersPaged", StoredProcedureType.SelectPaged)
                    .ForTable(_users)
                    .WithParameters(new List<SchemaColumn>()
                    {
                        new SchemaColumn()
                        {
                            Name = "Keywords",
                            DbType = DbType.String,
                            Length = "255"
                        },
                        new SchemaColumn()
                        {
                            Name = "RoleName",
                            DbType = DbType.String,
                            Length = "255"
                        }
                    }));

            // Indexes
            builder.IndexBuilder.CreateIndex(new SchemaIndex()
            {
                TableName = _users.Name,
                Columns = new string[]
                {
                    "UserName",
                    "NormalizedUserName",
                    "Email",
                    "NormalizedEmail",
                    "UserType"
                }
            });

        }

        void UserPhoto(ISchemaBuilder builder)
        {
            
            builder.TableBuilder.CreateTable(_userPhoto);

            builder.ProcedureBuilder
                .CreateDefaultProcedures(_userPhoto)
                .CreateProcedure(new SchemaProcedure("SelectUserPhotoByUserId", StoredProcedureType.SelectByKey)
                    .ForTable(_userPhoto)
                    .WithParameter(new SchemaColumn() {Name = "UserId", DbType = DbType.Int32}));

        }

        void UserBanner(ISchemaBuilder builder)
        {
            
            builder.TableBuilder.CreateTable(_userBanner);

            builder.ProcedureBuilder
                .CreateDefaultProcedures(_userBanner)
                .CreateProcedure(new SchemaProcedure("SelectUserBannerByUserId", StoredProcedureType.SelectByKey)
                    .ForTable(_userBanner)
                    .WithParameter(new SchemaColumn() {Name = "UserId", DbType = DbType.Int32}));

        }

        void UserData(ISchemaBuilder builder)
        {
            
            builder.TableBuilder.CreateTable(_userData);

            builder.ProcedureBuilder
                .CreateDefaultProcedures(_userData)
                .CreateProcedure(new SchemaProcedure("SelectUserDatumByUserId", StoredProcedureType.SelectByKey)
                    .ForTable(_userData)
                    .WithParameter(new SchemaColumn() {Name = "UserId", DbType = DbType.Int32}))
                .CreateProcedure(new SchemaProcedure("SelectUserDatumByKeyAndUserId", StoredProcedureType.SelectByKey)
                    .ForTable(_userData)
                    .WithParameters(new List<SchemaColumn>()
                    {
                        new SchemaColumn()
                        {
                            Name = "[Key]",
                            DbType = DbType.String,
                            Length = "255"
                        },
                        new SchemaColumn()
                        {
                            Name = "UserId",
                            DbType = DbType.Int32
                        }
                    }))

                .CreateProcedure(new SchemaProcedure("SelectUserDatumPaged", StoredProcedureType.SelectPaged)
                    .ForTable(_userData)
                    .WithParameters(new List<SchemaColumn>()
                    {
                        new SchemaColumn()
                        {
                            Name = "[Key]",
                            DbType = DbType.String,
                            Length = "255"
                        }
                    }));
            
        }

        void UserLogin(ISchemaBuilder builder)
        {

            builder.TableBuilder.CreateTable(_userLogin);

            builder.ProcedureBuilder
                .CreateDefaultProcedures(_userLogin)
                .CreateProcedure(new SchemaProcedure("SelectUserLoginsPaged", StoredProcedureType.SelectPaged)
                    .ForTable(_userLogin)
                    .WithParameters(new List<SchemaColumn>()
                    {
                        new SchemaColumn()
                        {
                            Name = "LoginProvider",
                            DbType = DbType.String,
                            Length = "128"
                        },
                         new SchemaColumn()
                        {
                            Name = "ProviderKey",
                            DbType = DbType.String,
                            Length = "128"
                        }
                    }));

        }

        async Task ConfigureDefaultUsers(ISetUpContext context, Action<string, string> reportError)
        {

            try
            {

                // create super user
                var result1 = await _userManager.CreateAsync(new User()
                {
                    Email = context.AdminEmail,
                    UserName = context.AdminUsername,
                    PhotoColor = _userColorProvider.GetColor(),
                    EmailConfirmed = true,
                    IsVerified = true,
                    IsVerifiedUpdatedUserId = 1,
                    IsVerifiedUpdatedDate = DateTimeOffset.UtcNow
                }, context.AdminPassword);
                if (!result1.Succeeded)
                {
                    foreach (var error in result1.Errors)
                    {
                        reportError(error.Code, error.Description);
                    }
                }

                // create default plato bot

                var result2 = await _userManager.CreateAsync(new User()
                {
                    Email = "bot@plato.com",
                    UserName = "PlatoBot",
                    DisplayName = "Plato Bot",
                    Biography = "I'm not a real person. I'm a bot that keeps an eye on things.",
                    EmailConfirmed = true,
                    IsVerified = true,
                    IsVerifiedUpdatedUserId = 1,
                    IsVerifiedUpdatedDate = DateTimeOffset.UtcNow,
                    PhotoUrl = "/images/bot.png",
                    PhotoColor = _userColorProvider.GetColor(),
                    UserType = UserType.Bot
                }, context.AdminPassword);
                if (!result2.Succeeded)
                {
                    foreach (var error in result2.Errors)
                    {
                        reportError(error.Code, error.Description);
                    }
                }


            }
            catch (Exception ex)
            {
                reportError(ex.Message, ex.StackTrace);
            }

        }

        async Task ConfigureSuperUser(ISetUpContext context, Action<string, string> reportError)
        {

            // Get newly installed administrator role
            var role = await _roleManager.FindByNameAsync(DefaultRoles.Administrator);

            // Get newly created administrator user
            var user = await _userManager.FindByNameAsync(context.AdminUsername);

            // Add our administrator user to the administrator role
            var dirty = false;
            if (role != null && user != null)
            {
                if (!await _userManager.IsInRoleAsync(user, role.Name))
                {
                    var result = await _userManager.AddToRoleAsync(user, role.Name);
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            reportError(error.Code, error.Description);
                        }
                    }
                    dirty = true;
                }

            }

            if (dirty)
            {
               var result = await _userManager.UpdateAsync(user);
               if (!result.Succeeded)
               {
                   foreach (var error in result.Errors)
                   {
                       reportError(error.Code, error.Description);
                   }
               }
            }

        }

        #endregion

    }

}
