﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using PlatoCore.Abstractions.SetUp;
using PlatoCore.Data.Schemas.Abstractions;
using PlatoCore.Security.Abstractions;

namespace Plato.Roles.Handlers
{

    public class SetUpEventHandler : BaseSetUpEventHandler
    {

        public const string Version = "1.0.1";
        
        // Roles schema
        private readonly SchemaTable _roles = new SchemaTable()
        {
            Name = "Roles",
            Columns = new List<SchemaColumn>()
            {
                new SchemaColumn()
                {
                    PrimaryKey = true,
                    Name = "Id",
                    DbType = DbType.Int32
                },
                new SchemaColumn()
                {
                    Name = "[Name]",
                    Length = "255",
                    DbType = DbType.String
                },
                new SchemaColumn()
                {
                    Name = "NormalizedName",
                    Length = "255",
                    DbType = DbType.String
                },
                new SchemaColumn()
                {
                    Name = "Description",
                    Length = "255",
                    DbType = DbType.String
                },
                new SchemaColumn()
                {
                    Name = "Claims",
                    Length = "max",
                    DbType = DbType.String
                },
                new SchemaColumn()
                {
                    Name = "ConcurrencyStamp",
                    Length = "255",
                    DbType = DbType.String
                },
                new SchemaColumn()
                {
                    Name = "CreatedUserId",
                    DbType = DbType.Int32
                },
                new SchemaColumn()
                {
                    Name = "CreatedDate",
                    DbType = DbType.DateTimeOffset
                },
                new SchemaColumn()
                {
                    Name = "ModifiedUserId",
                    DbType = DbType.Int32
                },
                new SchemaColumn()
                {
                    Name = "ModifiedDate",
                    DbType = DbType.DateTimeOffset,
                    Nullable = true
                }
            }
        };

        // User Roles Schema
        private readonly SchemaTable _userRoles = new SchemaTable()
        {
            Name = "UserRoles",
            Columns = new List<SchemaColumn>()
            {
                new SchemaColumn()
                {
                    PrimaryKey = true,
                    Name = "Id",
                    DbType = DbType.Int32
                },
                new SchemaColumn()
                {
                    Name = "UserId",
                    DbType = DbType.Int32
                },
                new SchemaColumn()
                {
                    Name = "RoleId",
                    DbType = DbType.Int32
                }
            }
        };

        private readonly IDefaultRolesManager _defaultRolesManager;
        private readonly ISchemaBuilder _schemaBuilder;
        private readonly ISchemaManager _schemaManager;

        public SetUpEventHandler(
            IDefaultRolesManager defaultRolesManager,
            ISchemaBuilder schemaBuilder,                        
            ISchemaManager schemaManager)
        {
            _defaultRolesManager = defaultRolesManager;
            _schemaBuilder = schemaBuilder;                        
            _schemaManager = schemaManager;
        }
        
        public override async Task SetUp(ISetUpContext context, Action<string, string> reportError)
        {
            
            using (var builder = _schemaBuilder)
            {

                // configure
                Configure(builder);

                // roles 
                Roles(builder);

                // user roles 
                UserRoles(builder);

                var errors = await _schemaManager.ExecuteAsync(builder.Statements);
                foreach (var error in errors)
                {
                    reportError(error, $"SetUp within {this.GetType().FullName} - {error}");
                }

            }

            // Install default roles & permissions on first set-up
            await _defaultRolesManager.InstallDefaultRolesAsync();

        }

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

        void Roles(ISchemaBuilder builder)
        {

            // create tables and default procedures
            builder.TableBuilder.CreateTable(_roles);
            builder.ProcedureBuilder.CreateDefaultProcedures(_roles)

                // create unique stored procedures
            
                .CreateProcedure(
                    new SchemaProcedure("SelectRoleByName", StoredProcedureType.SelectByKey)
                        .ForTable(_roles)
                        .WithParameter(new SchemaColumn()
                        {
                            Name = "[Name]",
                            DbType = DbType.String,
                            Length = "255"
                        }))

                .CreateProcedure(
                    new SchemaProcedure("SelectRoleByNameNormalized", StoredProcedureType.SelectByKey)
                        .ForTable(_roles)
                        .WithParameter(new SchemaColumn()
                        {
                            Name = "NormalizedName",
                            DbType = DbType.String,
                            Length = "255"
                        }))

                .CreateProcedure(new SchemaProcedure("SelectRolesPaged", StoredProcedureType.SelectPaged)
                    .ForTable(_roles)
                    .WithParameters(new List<SchemaColumn>()
                    {
                        new SchemaColumn()
                        {
                            Name = "Id",
                            DbType = DbType.Int32
                        },
                        new SchemaColumn()
                        {
                            Name = "Keywords",
                            DbType = DbType.String,
                            Length = "255"
                        }
                    }));

            // Indexes
            builder.IndexBuilder.CreateIndex(new SchemaIndex()
            {
                TableName = _roles.Name,
                Columns = new string[]
                {
                    "[Name]",
                    "NormalizedName"
                }
            });

        }

        void UserRoles(ISchemaBuilder builder)
        {

            // create tables 
            builder.TableBuilder.CreateTable(_userRoles);

            // Create procedures
            builder.ProcedureBuilder.CreateDefaultProcedures(_userRoles)

                // Overwrite our SelectUserRoleById created via CreateDefaultProcedures
                // above to also join and return all role data
                .CreateProcedure(
                    new SchemaProcedure(
                            $"SelectUserRoleById",
                            @"
                                SELECT 
                                    ur.Id, 
                                    ur.UserId, 
                                    r.Id AS RoleId, 
                                    r.[Name], 
                                    r.NormalizedName, 
                                    r.Description, 
                                    r.Claims, 
                                    r.CreatedDate, 
                                    r.CreatedUserId, 
                                    r.ModifiedDate, 
                                    r.ModifiedUserId, 
                                    r.ConcurrencyStamp 
                                FROM {prefix}_UserRoles ur WITH (nolock) 
                                INNER JOIN {prefix}_Roles r ON ur.RoleId = r.Id 
                                WHERE (
                                    ur.Id = @Id
                                )")
                        .ForTable(_userRoles)
                        .WithParameter(_userRoles.PrimaryKeyColumn))

                .CreateProcedure(
                    new SchemaProcedure("SelectRolesByUserId", @"
                            SELECT * FROM {prefix}_Roles WITH (nolock) WHERE Id IN (
	                            SELECT RoleId FROM {prefix}_UserRoles WITH (nolock) 
	                            WHERE (
	                               UserId = @UserId
	                            )
                            )
                        ")
                        .WithParameter(new SchemaColumn()
                        {
                            Name = "UserId",
                            DbType = DbType.Int32
                        }))

                .CreateProcedure(
                    new SchemaProcedure("SelectUserRolesByUserId", StoredProcedureType.SelectByKey)
                        .ForTable(_userRoles)
                        .WithParameter(new SchemaColumn()
                        {
                            Name = "UserId",
                            DbType = DbType.Int32
                        }))

                .CreateProcedure(new SchemaProcedure("SelectUserRolesPaged", StoredProcedureType.SelectPaged)
                    .ForTable(_userRoles)
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
                    }))

                .CreateProcedure(
                    new SchemaProcedure("DeleteUserRolesByUserId", StoredProcedureType.DeleteByKey)
                        .ForTable(_userRoles)
                        .WithParameter(new SchemaColumn()
                        {
                            Name = "UserId",
                            DbType = DbType.Int32
                        }))

                .CreateProcedure(
                    new SchemaProcedure("DeleteUserRoleByUserIdAndRoleId", StoredProcedureType.DeleteByKey)
                        .ForTable(_userRoles)
                        .WithParameters(new List<SchemaColumn>()
                        {
                            new SchemaColumn()
                            {
                                Name = "UserId",
                                DbType = DbType.Int32
                            },
                            new SchemaColumn()
                            {
                                Name = "RoleId",
                                DbType = DbType.Int32
                            }
                        }));

            // Indexes
            builder.IndexBuilder.CreateIndex(new SchemaIndex()
            {
                TableName = _userRoles.Name,
                Columns = new string[]
                {
                    "UserId",
                    "RoleId"
                }
            });

        }

    }

}
