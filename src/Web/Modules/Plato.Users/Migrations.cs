﻿using System.Data;
using System.Collections.Generic;
using PlatoCore.Data.Migrations.Abstractions;
using PlatoCore.Data.Schemas.Abstractions;

namespace Plato.Users
{
    public class Migrations : BaseMigrationProvider
    {

        private readonly ISchemaBuilder _schemaBuilder;

        public Migrations(ISchemaBuilder schemaBuilder)
        {
            _schemaBuilder = schemaBuilder;

            AvailableMigrations = new List<PreparedMigration>
            {
                new PreparedMigration()
                {
                    ModuleId = ModuleId,
                    Version = "1.0.1",
                    Statements = V_1_0_1()
                },
                new PreparedMigration()
                {
                    ModuleId = ModuleId,
                    Version = "1.0.8",
                    Statements = V_1_0_8()
                },
                new PreparedMigration()
                {
                    ModuleId = ModuleId,
                    Version = "1.0.9",
                    Statements = V_1_0_9()
                },
                new PreparedMigration()
                {
                    ModuleId = ModuleId,
                    Version = "1.1.0",
                    Statements = V_1_1_0()
                }
            };

        }

        // Migrations

        private ICollection<string> V_1_0_1()
        {

            var users = new SchemaTable()
            {
                Name = "Users",
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
                        Name = "PrimaryRoleId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "UserName",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "NormalizedUserName",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "Email",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "NormalizedEmail",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "EmailConfirmed",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "DisplayName",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "FirstName",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "LastName",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "Alias",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "PhotoUrl",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "PhotoColor",
                        Length = "6",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "SamAccountName",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "PasswordHash",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "PasswordExpiryDate",
                        DbType = DbType.DateTimeOffset,
                        Nullable = true
                    },
                    new SchemaColumn()
                    {
                        Name = "PasswordUpdatedDate",
                        DbType = DbType.DateTimeOffset,
                        Nullable = true
                    },
                    new SchemaColumn()
                    {
                        Name = "SecurityStamp",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "PhoneNumber",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "PhoneNumberConfirmed",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "TwoFactorEnabled",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "LockoutEnd",
                        Nullable = true,
                        DbType = DbType.DateTimeOffset
                    },
                    new SchemaColumn()
                    {
                        Name = "LockoutEnabled",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "AccessFailedCount",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "ResetToken",
                        DbType = DbType.String,
                        Length = "255"
                    },
                    new SchemaColumn()
                    {
                        Name = "ConfirmationToken",
                        DbType = DbType.String,
                        Length = "255"
                    },
                    new SchemaColumn()
                    {
                        Name = "ApiKey",
                        DbType = DbType.String,
                        Length = "255"
                    },
                    new SchemaColumn()
                    {
                        Name = "TimeZone",
                        DbType = DbType.String,
                        Length = "255"
                    },
                    new SchemaColumn()
                    {
                        Name = "ObserveDst",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "Culture",
                        DbType = DbType.String,
                        Length = "50"
                    },
                    new SchemaColumn()
                    {
                        Name = "Theme",
                        DbType = DbType.String,
                        Length = "50"
                    },
                    new SchemaColumn()
                    {
                        Name = "IpV4Address",
                        DbType = DbType.String,
                        Length = "20"
                    },
                    new SchemaColumn()
                    {
                        Name = "IpV6Address",
                        DbType = DbType.String,
                        Length = "50"
                    },
                    new SchemaColumn()
                    {
                        Name = "Biography",
                        DbType = DbType.String,
                        Length = "255"
                    },
                    new SchemaColumn()
                    {
                        Name = "Location",
                        DbType = DbType.String,
                        Length = "255"
                    },
                    new SchemaColumn()
                    {
                        Name = "[Url]",
                        DbType = DbType.String,
                        Length = "255"
                    },
                    new SchemaColumn()
                    {
                        Name = "Visits",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "VisitsUpdatedDate",
                        DbType = DbType.DateTimeOffset,
                    },
                    new SchemaColumn()
                    {
                        Name = "MinutesActive",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "MinutesActiveUpdatedDate",
                        DbType = DbType.DateTimeOffset,
                    },
                    new SchemaColumn()
                    {
                        Name = "Reputation",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "ReputationUpdatedDate",
                        DbType = DbType.DateTimeOffset,
                    },
                     new SchemaColumn()
                    {
                        Name = "[Rank]",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "RankUpdatedDate",
                        DbType = DbType.DateTimeOffset,
                    },
                    new SchemaColumn()
                    {
                        Name = "[Signature]",
                        DbType = DbType.String,
                        Length = "max"
                    },
                    new SchemaColumn()
                    {
                        Name = "SignatureHtml",
                        DbType = DbType.String,
                        Length = "max"
                    },
                    new SchemaColumn()
                    {
                        Name = "IsSpam",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsSpamUpdatedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "IsSpamUpdatedDate",
                        DbType = DbType.DateTimeOffset,
                        Nullable = true
                    },
                    new SchemaColumn()
                    {
                        Name = "IsVerified",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsVerifiedUpdatedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "IsVerifiedUpdatedDate",
                        DbType = DbType.DateTimeOffset,
                        Nullable = true
                    },
                    new SchemaColumn()
                    {
                        Name = "IsBanned",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsBannedUpdatedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "IsBannedUpdatedDate",
                        DbType = DbType.DateTimeOffset,
                        Nullable = true
                    },
                    new SchemaColumn()
                    {
                        Name = "IsBannedExpiryDate",
                        DbType = DbType.DateTimeOffset,
                        Nullable = true
                    },
                    new SchemaColumn()
                    {
                        Name = "UserType",
                        DbType = DbType.Int16
                    },
                    new SchemaColumn()
                    {
                        Name = "CreatedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "CreatedDate",
                        DbType = DbType.DateTimeOffset,
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
                    },
                    new SchemaColumn()
                    {
                        Name = "LastLoginDate",
                        DbType = DbType.DateTimeOffset,
                    }
                }
            };

            var output = new List<string>();

            using (var builder = _schemaBuilder)
            {

                builder
                    .Configure(options =>
                    {
                        options.ModuleName = ModuleId;
                        options.Version = "1.0.1";
                    });

                // ---------------
                // Users
                // ---------------

                // Add new columns to users table
                builder.TableBuilder.CreateTableColumns(new SchemaTable()
                {
                    Name = "Users",
                    Columns = new List<SchemaColumn>()
                    {
                        new SchemaColumn()
                        {
                            Name = "Biography",
                            DbType = DbType.String,
                            Length = "255"
                        },
                        new SchemaColumn()
                        {
                            Name = "Location",
                            DbType = DbType.String,
                            Length = "255"
                        },
                        new SchemaColumn()
                        {
                            Name = "[Url]",
                            DbType = DbType.String,
                            Length = "255"
                        }
                    }
                });

                // Drop & recreate InsertUpdateUser stored procedure
                builder.ProcedureBuilder.CreateProcedure(
                    new SchemaProcedure($"InsertUpdateUser",
                            StoredProcedureType.InsertUpdate)
                        .ForTable(users));

                // Add builder results to output
                output.AddRange(builder.Statements);

            }

            return output;

        }

        private ICollection<string> V_1_0_8()
        {

            // Add IsStaff fields

            var users = new SchemaTable()
            {
                Name = "Users",
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
                        Name = "PrimaryRoleId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "UserName",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "NormalizedUserName",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "Email",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "NormalizedEmail",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "EmailConfirmed",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "DisplayName",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "FirstName",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "LastName",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "Alias",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "PhotoUrl",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "PhotoColor",
                        Length = "6",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "SamAccountName",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "PasswordHash",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "PasswordExpiryDate",
                        DbType = DbType.DateTimeOffset,
                        Nullable = true
                    },
                    new SchemaColumn()
                    {
                        Name = "PasswordUpdatedDate",
                        DbType = DbType.DateTimeOffset,
                        Nullable = true
                    },
                    new SchemaColumn()
                    {
                        Name = "SecurityStamp",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "PhoneNumber",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "PhoneNumberConfirmed",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "TwoFactorEnabled",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "LockoutEnd",
                        Nullable = true,
                        DbType = DbType.DateTimeOffset
                    },
                    new SchemaColumn()
                    {
                        Name = "LockoutEnabled",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "AccessFailedCount",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "ResetToken",
                        DbType = DbType.String,
                        Length = "255"
                    },
                    new SchemaColumn()
                    {
                        Name = "ConfirmationToken",
                        DbType = DbType.String,
                        Length = "255"
                    },
                    new SchemaColumn()
                    {
                        Name = "ApiKey",
                        DbType = DbType.String,
                        Length = "255"
                    },
                    new SchemaColumn()
                    {
                        Name = "TimeZone",
                        DbType = DbType.String,
                        Length = "255"
                    },
                    new SchemaColumn()
                    {
                        Name = "ObserveDst",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "Culture",
                        DbType = DbType.String,
                        Length = "50"
                    },
                    new SchemaColumn()
                    {
                        Name = "Theme",
                        DbType = DbType.String,
                        Length = "255"
                    },
                    new SchemaColumn()
                    {
                        Name = "IpV4Address",
                        DbType = DbType.String,
                        Length = "20"
                    },
                    new SchemaColumn()
                    {
                        Name = "IpV6Address",
                        DbType = DbType.String,
                        Length = "50"
                    },
                    new SchemaColumn()
                    {
                        Name = "Biography",
                        DbType = DbType.String,
                        Length = "150"
                    },
                    new SchemaColumn()
                    {
                        Name = "Location",
                        DbType = DbType.String,
                        Length = "100"
                    },
                    new SchemaColumn()
                    {
                        Name = "[Url]",
                        DbType = DbType.String,
                        Length = "255"
                    },
                    new SchemaColumn()
                    {
                        Name = "Visits",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "VisitsUpdatedDate",
                        DbType = DbType.DateTimeOffset,
                    },
                    new SchemaColumn()
                    {
                        Name = "MinutesActive",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "MinutesActiveUpdatedDate",
                        DbType = DbType.DateTimeOffset,
                    },
                    new SchemaColumn()
                    {
                        Name = "Reputation",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "ReputationUpdatedDate",
                        DbType = DbType.DateTimeOffset,
                    },
                    new SchemaColumn()
                    {
                        Name = "[Rank]",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "RankUpdatedDate",
                        DbType = DbType.DateTimeOffset,
                    },
                    new SchemaColumn()
                    {
                        Name = "[Signature]",
                        DbType = DbType.String,
                        Length = "max"
                    },
                    new SchemaColumn()
                    {
                        Name = "SignatureHtml",
                        DbType = DbType.String,
                        Length = "max"
                    },
                    new SchemaColumn()
                    {
                        Name = "IsSpam",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsSpamUpdatedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "IsSpamUpdatedDate",
                        DbType = DbType.DateTimeOffset,
                        Nullable = true
                    },
                    new SchemaColumn()
                    {
                        Name = "IsVerified",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsVerifiedUpdatedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "IsVerifiedUpdatedDate",
                        DbType = DbType.DateTimeOffset,
                        Nullable = true
                    },
                    new SchemaColumn()
                    {
                        Name = "IsStaff",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsStaffUpdatedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "IsStaffUpdatedDate",
                        DbType = DbType.DateTimeOffset,
                        Nullable = true
                    },
                    new SchemaColumn()
                    {
                        Name = "IsBanned",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsBannedUpdatedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "IsBannedUpdatedDate",
                        DbType = DbType.DateTimeOffset,
                        Nullable = true
                    },
                    new SchemaColumn()
                    {
                        Name = "IsBannedExpiryDate",
                        DbType = DbType.DateTimeOffset,
                        Nullable = true
                    },
                    new SchemaColumn()
                    {
                        Name = "UserType",
                        DbType = DbType.Int16
                    },
                    new SchemaColumn()
                    {
                        Name = "CreatedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "CreatedDate",
                        DbType = DbType.DateTimeOffset,
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
                    },
                    new SchemaColumn()
                    {
                        Name = "LastLoginDate",
                        DbType = DbType.DateTimeOffset,
                    }
                }
            };

            var output = new List<string>();

            using (var builder = _schemaBuilder)
            {

                builder
                    .Configure(options =>
                    {
                        options.ModuleName = ModuleId;
                        options.Version = "1.0.8";
                    });

                // ---------------
                // Users
                // ---------------

                // Add new columns to users table
                builder.TableBuilder.CreateTableColumns(new SchemaTable()
                {
                    Name = "Users",
                    Columns = new List<SchemaColumn>()
                    {
                        new SchemaColumn()
                        {
                            Name = "IsStaff",
                            DbType = DbType.Boolean
                        },
                        new SchemaColumn()
                        {
                            Name = "IsStaffUpdatedUserId",
                            DbType = DbType.Int32
                        },
                        new SchemaColumn()
                        {
                            Name = "IsStaffUpdatedDate",
                            DbType = DbType.DateTimeOffset,
                            Nullable = true
                        }
                    }
                });

                // Drop & recreate InsertUpdateUser stored procedure
                builder.ProcedureBuilder.CreateProcedure(
                    new SchemaProcedure($"InsertUpdateUser",
                            StoredProcedureType.InsertUpdate)
                        .ForTable(users));

                // Add builder results to output
                output.AddRange(builder.Statements);

            }

            return output;

        }

        private ICollection<string> V_1_0_9()
        {

            var users = new SchemaTable()
            {
                Name = "Users",
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
                        Name = "PrimaryRoleId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "UserName",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "NormalizedUserName",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "Email",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "NormalizedEmail",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "EmailConfirmed",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "DisplayName",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "FirstName",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "LastName",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "Alias",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "PhotoUrl",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "PhotoColor",
                        Length = "6",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "SamAccountName",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "PasswordHash",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "PasswordExpiryDate",
                        DbType = DbType.DateTimeOffset,
                        Nullable = true
                    },
                    new SchemaColumn()
                    {
                        Name = "PasswordUpdatedDate",
                        DbType = DbType.DateTimeOffset,
                        Nullable = true
                    },
                    new SchemaColumn()
                    {
                        Name = "SecurityStamp",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "PhoneNumber",
                        Length = "255",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "PhoneNumberConfirmed",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "TwoFactorEnabled",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "LockoutEnd",
                        Nullable = true,
                        DbType = DbType.DateTimeOffset
                    },
                    new SchemaColumn()
                    {
                        Name = "LockoutEnabled",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "AccessFailedCount",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "ResetToken",
                        DbType = DbType.String,
                        Length = "255"
                    },
                    new SchemaColumn()
                    {
                        Name = "ConfirmationToken",
                        DbType = DbType.String,
                        Length = "255"
                    },
                    new SchemaColumn()
                    {
                        Name = "ApiKey",
                        DbType = DbType.String,
                        Length = "255"
                    },
                    new SchemaColumn()
                    {
                        Name = "TimeZone",
                        DbType = DbType.String,
                        Length = "255"
                    },
                    new SchemaColumn()
                    {
                        Name = "ObserveDst",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "Culture",
                        DbType = DbType.String,
                        Length = "50"
                    },
                    new SchemaColumn()
                    {
                        Name = "Theme",
                        DbType = DbType.String,
                        Length = "255"
                    },
                    new SchemaColumn()
                    {
                        Name = "IpV4Address",
                        DbType = DbType.String,
                        Length = "20"
                    },
                    new SchemaColumn()
                    {
                        Name = "IpV6Address",
                        DbType = DbType.String,
                        Length = "50"
                    },
                    new SchemaColumn()
                    {
                        Name = "Biography",
                        DbType = DbType.String,
                        Length = "150"
                    },
                    new SchemaColumn()
                    {
                        Name = "Location",
                        DbType = DbType.String,
                        Length = "100"
                    },
                    new SchemaColumn()
                    {
                        Name = "[Url]",
                        DbType = DbType.String,
                        Length = "255"
                    },
                    new SchemaColumn()
                    {
                        Name = "Visits",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "VisitsUpdatedDate",
                        DbType = DbType.DateTimeOffset,
                    },
                    new SchemaColumn()
                    {
                        Name = "MinutesActive",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "MinutesActiveUpdatedDate",
                        DbType = DbType.DateTimeOffset,
                    },
                    new SchemaColumn()
                    {
                        Name = "Reputation",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "ReputationUpdatedDate",
                        DbType = DbType.DateTimeOffset,
                    },
                     new SchemaColumn()
                    {
                        Name = "[Rank]",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "RankUpdatedDate",
                        DbType = DbType.DateTimeOffset,
                    },
                    new SchemaColumn()
                    {
                        Name = "[Signature]",
                        DbType = DbType.String,
                        Length = "max"
                    },
                    new SchemaColumn()
                    {
                        Name = "SignatureHtml",
                        DbType = DbType.String,
                        Length = "max"
                    },
                    new SchemaColumn()
                    {
                        Name = "IsSpam",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsSpamUpdatedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "IsSpamUpdatedDate",
                        DbType = DbType.DateTimeOffset,
                        Nullable = true
                    },
                    new SchemaColumn()
                    {
                        Name = "IsVerified",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsVerifiedUpdatedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "IsVerifiedUpdatedDate",
                        DbType = DbType.DateTimeOffset,
                        Nullable = true
                    },
                    new SchemaColumn()
                    {
                        Name = "IsStaff",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsStaffUpdatedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "IsStaffUpdatedDate",
                        DbType = DbType.DateTimeOffset,
                        Nullable = true
                    },
                    new SchemaColumn()
                    {
                        Name = "IsBanned",
                        DbType = DbType.Boolean
                    },
                    new SchemaColumn()
                    {
                        Name = "IsBannedUpdatedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "IsBannedUpdatedDate",
                        DbType = DbType.DateTimeOffset,
                        Nullable = true
                    },
                    new SchemaColumn()
                    {
                        Name = "IsBannedExpiryDate",
                        DbType = DbType.DateTimeOffset,
                        Nullable = true
                    },
                    new SchemaColumn()
                    {
                        Name = "UserType",
                        DbType = DbType.Int16
                    },
                    new SchemaColumn()
                    {
                        Name = "CreatedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "CreatedDate",
                        DbType = DbType.DateTimeOffset,
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
                    },
                    new SchemaColumn()
                    {
                        Name = "LastLoginDate",
                        DbType = DbType.DateTimeOffset,
                    }
                }
            };

            var userLogin = new SchemaTable()
            {
                Name = "UserLogins",
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
                        Name = "LoginProvider",
                        Length = "128",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "ProviderKey",
                        Length = "128",
                        DbType = DbType.String
                    },
                    new SchemaColumn()
                    {
                        Name = "ProviderDisplayName",
                        Length = "max",
                        DbType = DbType.String
                    },                    
                    new SchemaColumn()
                    {
                        Name = "CreatedDate",
                        DbType = DbType.DateTimeOffset
                    }
                }
            };

            var output = new List<string>();
            using (var builder = _schemaBuilder)
            {

                builder.Configure(options =>
                    {
                        options.ModuleName = ModuleId;
                        options.Version = "1.0.9";
                        options.DropTablesBeforeCreate = true;
                        options.DropProceduresBeforeCreate = true;
                    });

                builder.TableBuilder.CreateTable(userLogin);

                builder.ProcedureBuilder
                    .CreateDefaultProcedures(userLogin)
                    .CreateProcedure(new SchemaProcedure("SelectUserLoginsPaged", StoredProcedureType.SelectPaged)
                        .ForTable(userLogin)
                        .WithParameters(new List<SchemaColumn>() {
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
                        }))

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
                        .ForTable(users)
                        .WithParameter(users.PrimaryKeyColumn));

                // Add builder results to output
                output.AddRange(builder.Statements);

            }

            return output;

        }

        private ICollection<string> V_1_1_0()
        {

            // Plato.USers 1.1.0 adds indexes to the users table to help with performance
            // We also drop and recreate the InsertUpdateUser stored procedure using the latest table schema

            var output = new List<string>();
            using (var builder = _schemaBuilder)
            {

                builder.Configure(options =>
                {
                    options.ModuleName = ModuleId;
                    options.Version = "1.1.0";
                    options.DropTablesBeforeCreate = true;
                    options.DropProceduresBeforeCreate = true;
                });

                // Indexes
                builder.IndexBuilder.CreateIndex(new SchemaIndex()
                {
                    TableName = "Users",
                    Columns = new string[]
                    {
                        "UserName",
                        "NormalizedUserName",
                        "Email",
                        "NormalizedEmail",
                        "UserType"
                    }
                });

                // Drop & recreate InsertUpdateUser stored procedure
                builder.ProcedureBuilder.CreateProcedure(
                    new SchemaProcedure($"InsertUpdateUser",
                            StoredProcedureType.InsertUpdate)
                        .ForTable(UserTables.Users));

                // Add builder results to output
                output.AddRange(builder.Statements);

            }

            return output;

        }

    }

}
