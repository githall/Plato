﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Data.Schemas.Abstractions;
using Plato.Internal.Text;

namespace Plato.Internal.Data.Schemas
{

    public class SchemaBuilder : ISchemaBuilder, IDisposable
    {
        private readonly string _tablePrefix;

        private List<string> _statements;
        private List<Exception> _errors;

        public List<Exception> Errors => _errors;

        public List<string> Statements
        {
            get => _statements ?? (_statements = new List<string>());
            set => _statements = value;
        }

        private readonly IDbContext _dbContext;

        private readonly Pluralizer _pluralizer;

        public SchemaBuilder(IDbContext dbContext)
        {
            _tablePrefix = dbContext.Configuration.TablePrefix;
            _dbContext = dbContext;

            _statements = new List<string>();
            _errors = new List<Exception>();
            _pluralizer = new Pluralizer();

        }

        private readonly string _newLine = Environment.NewLine;

        #region "Implementation"

        private SchemaBuilderOptions _options;

        public ISchemaBuilder Configure(Action<SchemaBuilderOptions> configure)
        {
            _options = new SchemaBuilderOptions();
            configure(_options);
            return this;
        }
        
        public ISchemaBuilder CreateTable(SchemaTable table)
        {

            var tableName = GetTableName(table.Name);
            
            if (_options.DropTablesBeforeCreate)
            {
                DropTable(table);
            }

            var sb = new StringBuilder();
            sb.Append("CREATE TABLE ")
                .Append(tableName);
            
            if (table.Columns.Count > 0)
            {
                sb.Append("(");
                sb.Append(System.Environment.NewLine);

                var i = 0;
                var primaryKey = string.Empty;
                foreach (var column in table.Columns)
                {
                    if (column.PrimaryKey)
                        primaryKey = column.Name;
                    sb.Append(DescribeTableColumn(column));
                    if (!string.IsNullOrEmpty(primaryKey))
                    {
                        sb.Append(",");
                    }
                    else
                    {
                        if (i < table.Columns.Count)
                            sb.Append(",");
                    }
                    sb.Append(System.Environment.NewLine);
                    i += 1;
                }

                if (!string.IsNullOrEmpty(primaryKey))
                {
                    sb.Append("CONSTRAINT PK_")
                        .Append(tableName)
                        .Append("_")
                        .Append(primaryKey)
                        .Append(" PRIMARY KEY CLUSTERED ( ")
                        .Append(primaryKey)
                        .Append(" )");
                }

                sb.Append(System.Environment.NewLine);
                sb.Append(");");
            
            }

            sb.Append("SELECT 1 AS MigrationId;");

            CreateStatement(sb.ToString());

            return this;
        }
        
        public ISchemaBuilder DropTable(SchemaTable table)
        {
            var tableName = GetTableName(table.Name);

            // drop tabl;e ensuring it exists first
            var sb = new StringBuilder();
            sb.Append("IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'")
                .Append(tableName)
                .Append("'")
                .Append(_newLine)
                .Append("BEGIN")
                .Append(_newLine)
                .Append("DROP TABLE ")
                .Append(tableName)
                .Append(_newLine)
                .Append("END");

            CreateStatement(sb.ToString());
            return this;
        }
        
        public ISchemaBuilder AlterTableColumns(SchemaTable table)
        {

            var tableName = GetTableName(table.Name);
            var sb = new StringBuilder();
            if (table.Columns.Count > 0)
            {
                foreach (var column in table.Columns)
                {
                    sb.Append("ALTER TABLE ")
                        .Append(tableName)
                        .Append(" ADD ")
                        .Append(DescribeTableColumn(column))
                        .Append(";")
                        .Append(System.Environment.NewLine);
                }
              
                sb.Append("SELECT 1 AS MigrationId;");

                CreateStatement(sb.ToString());

            }
            return this;
        }

        public ISchemaBuilder DropTableColumns(SchemaTable table)
        {
            var tableName = GetTableName(table.Name);
            var sb = new StringBuilder();
            if (table.Columns.Count > 0)
            {
                foreach (var column in table.Columns)
                {
                    sb.Append("ALTER TABLE ")
                        .Append(tableName)
                        .Append(" DROP ")
                        .Append(column.Name)
                        .Append(";");
                }

                CreateStatement(sb.ToString());

            }
            return this;
        }

        public ISchemaBuilder CreateDefaultProcedures(SchemaTable table)
        {


            // select * from table
            CreateProcedure(new SchemaProcedure($"Select{table.NameNormalized}", StoredProcedureType.Select)
                .ForTable(table));

            // select * from table where primaryKey = @primaryKey
            CreateProcedure(
                new SchemaProcedure(
                        $"Select{_pluralizer.Singularize(table.Name)}By{table.PrimaryKeyColumn.NameNormalized}",
                        StoredProcedureType.SelectByKey)
                    .ForTable(table)
                    .WithParameter(table.PrimaryKeyColumn));

            // delete from table where primaryKey = @primaryKey
            CreateProcedure(
                new SchemaProcedure(
                        $"Delete{_pluralizer.Singularize(table.Name)}By{table.PrimaryKeyColumn.NameNormalized}",
                        StoredProcedureType.DeleteByKey)
                    .ForTable(table)
                    .WithParameter(table.PrimaryKeyColumn));

            // insert / update by primary key
            CreateProcedure(
                new SchemaProcedure($"InsertUpdate{_pluralizer.Singularize(table.Name)}",
                        StoredProcedureType.InsertUpdate)
                    .ForTable(table));

            return this;

        }

        public ISchemaBuilder DropProcedure(SchemaProcedure procedure)
        {
            var sb = new StringBuilder();
            sb.Append("DROP PROCEDURE ")
                .Append(GetProcedureName(procedure.Name))
                .Append(";")
                .Append(_newLine);
            CreateStatement(sb.ToString());
            return this;

        }

        public ISchemaBuilder CreateProcedure(SchemaProcedure procedure)
        {
            
            if (_options.DropProceduresBeforeCreate)
            {
                DropProcedure(procedure);
            }
            
            // TODO: Refactor to avoid switch statement 
            // Create common command classes to represent each procedure type
            // Avoids enum and makes it more extensible for future additins
            // InsertUpdateProcedure : IProcedureCommand,
            // SelectByKeyProcedure : IProcedureCommand
            // DeleteByKey : IProcedureCommand etc
            var statement = string.Empty;
            switch (procedure.ProcedureType)
            {
                case StoredProcedureType.InsertUpdate:
                    statement = BuildInsertUpdateProcedure(procedure);
                    break;
                case StoredProcedureType.Select:
                    statement = BuildSelectProcedure(procedure);
                    break;
                case StoredProcedureType.SelectByKey:
                    statement = BuildSelectByProcedure(procedure);
                    break;
                case StoredProcedureType.SelectPaged:
                    statement = BuildSelectPagedProcedure(procedure);
                    break;
                case StoredProcedureType.DeleteByKey:
                    statement = BuildDeleteByKeyProcedure(procedure);
                    break;
            }

            CreateStatement(statement);

            return this;

        }

        public ISchemaBuilder CreateStatement(string statement)
        {
            var notNull = !string.IsNullOrEmpty(statement);
            var notPresent = !_statements.Contains(statement);
            if (notNull && notPresent)
                _statements.Add(statement);
            return this;
        }
        
        public ISchemaBuilder ApplySchema()
        {

            using (var context = _dbContext)
            {
                foreach (var statement in _statements)
                {
                    try
                    {
                        context.Execute(CommandType.Text, statement);
                    }
                    catch (Exception ex)
                    {
                        _errors.Add(ex);
                    }
                }
            }
            
            return this;

        }

        public async Task<ISchemaBuilder> ApplySchemaAsync()
        {

            using (var context = _dbContext)
            {
                foreach (var statement in _statements)
                {
                    try
                    {
                        await context.ExecuteAsync(CommandType.Text, statement);
                    }
                    catch (Exception ex)
                    {
                        if (_errors == null)
                            _errors = new List<Exception>();
                        _errors.Add(ex);
                    }
                }

            }

            return this;
        }

        public void Dispose()
        {
            _statements = null;
            _errors = null;
        }

        #endregion

        #region "Private Methods"

        private string BuildSelectProcedure(SchemaProcedure procedure)
        {

            var sb = new StringBuilder();

            sb.Append("CREATE PROCEDURE ")
                .Append(GetProcedureName(procedure.Name))
                .Append(_newLine)
                .Append("AS")
                .Append(_newLine)
                .Append(_newLine);

            sb.Append("SET NOCOUNT ON")
                .Append(_newLine)
                .Append(_newLine);
            
            sb.Append(GetProcedurePlaceHolderComment())
                .Append(_newLine)
                .Append(_newLine);

            sb.Append("SELECT * FROM ")
                .Append(GetTableName(procedure.Table.Name))
                .Append(" WITH (nolock)");

            return sb.ToString();

        }

        private string BuildSelectByProcedure(SchemaProcedure procedure)
        {

            if (procedure.Parameters == null)
                throw new Exception($"Attempting to create '{GetProcedureName(procedure.Name)}' procedure but no parameters have been defined. Use the WithParameter or WithParameter methods on the SchemaProcedure object.");

            var sb = new StringBuilder();
            sb.Append("CREATE PROCEDURE ")
                .Append(GetProcedureName(procedure.Name))
                .Append(" (")
                .Append(_newLine);

            var i = 0;
            foreach (var parameter in procedure.Parameters)
            {
                sb
                    .Append("   ")
                    .Append("@")
                    .Append(parameter.NameNormalized)
                    .Append(" ")
                    .Append(parameter.DbTypeNormalized)
                    .Append(i < procedure.Parameters.Count - 1 ? "," : "")
                    .Append(_newLine);
                i += 1;
            }

            sb
                .Append(") AS")
                .Append(_newLine)
                .Append("SET NOCOUNT ON")
                .Append(_newLine)
                .Append(_newLine);
            
            sb.Append(GetProcedurePlaceHolderComment())
                .Append(_newLine)
                .Append(_newLine);

            sb.Append("SELECT * FROM ")
                .Append(GetTableName(procedure.Table.Name))
                .Append(" WITH (nolock) ")
                .Append(_newLine)
                .Append("WHERE (")
                .Append(_newLine);

            i = 0;
            foreach (var parameter in procedure.Parameters)
            {
                sb
                    .Append("   ")
                    .Append(parameter.Name)
                    .Append(" = @")
                    .Append(parameter.NameNormalized)
                    .Append(i < procedure.Parameters.Count - 1 ? " AND " : "")
                    .Append(_newLine);
                i += 1;
            }
            
            sb.Append(")");

            return sb.ToString();

        }

        public string BuildSelectPagedProcedure(SchemaProcedure procedure)
        {

            /* Generates a stored procedure similar to...
             
                CREATE  PROCEDURE SelectUsersPaged
                (    
	                @PageIndex int = 1,
                    @PageSize int = 10,
	                @SqlStartId nvarchar(max) = 'SELECT @start_id_out = Id FROM Plato_Users ORDER BY Id',
	                @SqlPopulate nvarchar(max) = 'SELECT * FROM Plato_Users WHERE Id >= @start_id_in ORDER BY Id',
	                @SqlCount nvarchar(max) = 'SELECT COUNT(Id) FROM Plato_Users',	                
	                @Id int = 0,
	                @UserName nvarchar(255) = '',
	                @Email nvarchar(255) = ''
                )
                AS

                DECLARE @first_id int, 
	                @start_row int

                DECLARE @SqlParams nvarchar(max) = '@Id int, @UserName nvarchar(255), @Email nvarchar(255)';

                -- set start row pageSize * pageIndex - pageSize - 1
                -- 1 * 5  = 1, 2 * 5 = 6, 3 * 5 = 11
                -- 1 * 10  = 1, 2 * 10 = 11, 3 * 10 = 21
                SET @start_row = 1;
                IF (@PageIndex > 1)
	                SET @start_row = (
		                (@PageIndex * @PageSize) - ( @PageSize - 1 )
	                )

                -- Get the first row for our page of records
                SET ROWCOUNT @start_row
                DECLARE @parms nvarchar(100);

                -- get the first Id
                SET @parms = '@start_id_out int OUTPUT,' + + @SqlParams;  
                EXECUTE sp_executesql  @SqlStartId, @parms, 
	                @start_id_out = @first_id OUTPUT,
	                @Id = 1,
	                @UserName = '',
	                @Email = ''

                -- set to our page size
                SET ROWCOUNT @PageSize

                -- add our start parameter to the start
                SET @SqlParams = '@start_id_in int,' + @SqlParams;

                -- get all records >= @first_id
                EXECUTE sp_executesql @SqlPopulate, @SqlParams, 
	                @start_id_in = @first_id,
	                @Id = 1,
	                @UserName = '',
	                @Email = ''

                SET ROWCOUNT 0;

                -- total count
                IF (@SqlCount <> '')
	                EXECUTE sp_executesql @SqlCount, @SqlParams, 
		                @start_id_in = @first_id,
		                @Id = 1,
		                @UserName = '',
		                @Email = ''


             */

            if (procedure.Parameters == null)
                throw new Exception($"Attempting to create '{GetProcedureName(procedure.Name)}' procedure but no parameters have been defined. Use the WithParameter or WithParameter methods on the SchemaProcedure object.");

            var sb = new StringBuilder();
            sb.Append("CREATE PROCEDURE ")
                .Append(GetProcedureName(procedure.Name))
                .Append(" (")
                .Append(_newLine);

            sb.Append("   ")
                .Append("@PageIndex int = 1,")
                .Append(_newLine);
            sb.Append("   ")
                .Append("@PageSize int = 10,")
                .Append(_newLine);
            sb.Append("   ")
                .Append("@SqlStartId nvarchar(max),")
                .Append(_newLine);
            sb.Append("   ")
                .Append("@SqlPopulate nvarchar(max),")
                .Append(_newLine);
            sb.Append("   ")
                .Append("@SqlCount nvarchar(max),")
                .Append(_newLine);
            var i = 0;
            foreach (var parameter in procedure.Parameters)
            {
                sb
                    .Append("   ")
                    .Append("@")
                    .Append(parameter.NameNormalized)
                    .Append(" ")
                    .Append(parameter.DbTypeNormalized)
                    .Append(i < procedure.Parameters.Count - 1 ? "," : "")
                    .Append(_newLine);
                i += 1;
            }

            sb
                .Append(") AS")
                .Append(_newLine)
                .Append("SET NOCOUNT ON")
                .Append(_newLine)
                .Append(_newLine);
            
            sb.Append(GetProcedurePlaceHolderComment())
                .Append(_newLine)
                .Append(_newLine);

            sb.Append("DECLARE @first_id int, ")
                .Append("@start_row int")
                .Append(_newLine)
                .Append(_newLine);
            
            sb.Append("DECLARE @SqlParams nvarchar(max) = '");
            i = 0;
            foreach (var parameter in procedure.Parameters)
            {
                sb.Append("@")
                    .Append(parameter.NameNormalized)
                    .Append(" ")
                    .Append(parameter.DbTypeNormalized)
                    .Append(i < procedure.Parameters.Count - 1 ? ", " : "");
                i += 1;
            }
            sb.Append("';")
                .Append(_newLine)
                .Append(_newLine);
            

            sb.Append("-- set start row pageSize * pageIndex - pageSize - 1")
                .Append(_newLine)
                .Append("-- 1 * 5 = 1, 2 * 5 = 6, 3 * 5 = 11")
                .Append(_newLine)
                .Append("-- 1 * 10 = 1, 2 * 10 = 11, 3 * 10 = 21")
                .Append(_newLine);

            sb.Append("SET @start_row = 1;")
                .Append(_newLine)
                .Append("IF (@PageIndex > 1)")
                .Append(_newLine)
                .Append("   ")
                .Append("SET @start_row = (")
                .Append(_newLine)
                .Append("       ")
                .Append("(@PageIndex * @PageSize) - (@PageSize - 1)")
                .Append(_newLine)
                .Append("   ")
                .Append(")")
                .Append(_newLine)
                .Append(_newLine);

            sb.Append("-- Get the first row for our page of records")
                .Append(_newLine)
                .Append("SET ROWCOUNT @start_row;")
                .Append(_newLine)
                .Append("DECLARE @parms nvarchar(100);")
                .Append(_newLine)
                .Append(_newLine);
            
            sb.Append("-- get the first Id")
                .Append(_newLine)
                .Append("SET @parms = '@start_id_out int OUTPUT,' + @SqlParams;")
                .Append(_newLine)
                .Append("EXECUTE sp_executesql  @SqlStartId, @parms, ")
                .Append(_newLine)
                .Append("   ")
                .Append("@start_id_out = @first_id OUTPUT,")
                .Append(_newLine);
            i = 0;
            foreach (var parameter in procedure.Parameters)
            {
                sb
                    .Append("   ")
                    .Append("@")
                    .Append(parameter.NameNormalized)
                    .Append(" = ")
                    .Append(parameter.DefaultValueNormalizsed)
                    .Append(i < procedure.Parameters.Count - 1 ? "," : ";")
                    .Append(_newLine);
                i += 1;
            }
            
            sb.Append(_newLine)
                .Append("-- set to our page size")
                .Append(_newLine)
                .Append("SET ROWCOUNT @PageSize;")
                .Append(_newLine)
                .Append(_newLine);

            sb.Append("-- add our start parameter to the start")
                .Append(_newLine)
                .Append("SET @SqlParams = '@start_id_in int,' + @SqlParams;")
                .Append(_newLine)
                .Append(_newLine);

            sb.Append("-- get all records >= @first_id")
                .Append(_newLine)
                .Append("EXECUTE sp_executesql @SqlPopulate, @SqlParams,")
                .Append(_newLine)
                .Append("   ")
                .Append("@start_id_in = @first_id,")
                .Append(_newLine);
            i = 0;
            foreach (var parameter in procedure.Parameters)
            {
                sb
                    .Append("   ")
                    .Append("@")
                    .Append(parameter.NameNormalized)
                    .Append(" = ")
                    .Append(parameter.DefaultValueNormalizsed)
                    .Append(i < procedure.Parameters.Count - 1 ? "," : ";")
                    .Append(_newLine);
                i += 1;
            }

            sb.Append(_newLine)
                .Append("SET ROWCOUNT 0;")
                .Append(_newLine)
                .Append(_newLine);

            sb.Append("-- total count")
                .Append(_newLine)
                .Append("IF(@SqlCount <> '')")
                .Append(_newLine)
                .Append("EXECUTE sp_executesql @SqlCount, @SqlParams,")
                .Append(_newLine)
                .Append("   ")
                .Append("@start_id_in = @first_id,")
                .Append(_newLine);
            i = 0;
            foreach (var parameter in procedure.Parameters)
            {
                sb
                    .Append("   ")
                    .Append("@")
                    .Append(parameter.NameNormalized)
                    .Append(" = ")
                    .Append(parameter.DefaultValueNormalizsed)
                    .Append(i < procedure.Parameters.Count - 1 ? "," : ";")
                    .Append(_newLine);
                i += 1;
            }
            
            return sb.ToString();

        }

        private string BuildDeleteByKeyProcedure(SchemaProcedure procedure)
        {

            if (procedure.Parameters == null)
                throw new Exception($"Attempting to create '{GetProcedureName(procedure.Name)}' procedure but no parameters have been defined. Use the WithParameter or WithParameter methods on the SchemaProcedure object.");

            var sb = new StringBuilder();
            sb.Append("CREATE PROCEDURE ")
                .Append(GetProcedureName(procedure.Name))
                .Append(" (")
                .Append(_newLine);

            var i = 0;
            foreach (var parameter in procedure.Parameters)
            {
                sb
                    .Append("   ")
                    .Append("@")
                    .Append(parameter.NameNormalized)
                    .Append(" ")
                    .Append(parameter.DbTypeNormalized)
                    .Append(i < procedure.Parameters.Count - 1 ? "," : "")
                    .Append(_newLine);
                i += 1;
            }

            sb
                .Append(") AS")
                .Append(_newLine)
                .Append("SET NOCOUNT ON")
                .Append(_newLine)
                .Append(_newLine);

            sb.Append(GetProcedurePlaceHolderComment())
                .Append(_newLine)
                .Append(_newLine);

            sb.Append("DELETE FROM ")
                .Append(GetTableName(procedure.Table.Name))
                .Append(_newLine)
                .Append("WHERE (")
                .Append(_newLine);

            i = 0;
            foreach (var parameter in procedure.Parameters)
            {
                sb
                    .Append("   ")
                    .Append(parameter.Name)
                    .Append(" = @")
                    .Append(parameter.NameNormalized)
                    .Append(i < procedure.Parameters.Count - 1 ? " AND " : "")
                    .Append(_newLine);
                i += 1;
            }

            sb.Append(")");

            return sb.ToString();

        }

        private string BuildInsertUpdateProcedure(SchemaProcedure procedure)
        {

            if (procedure == null)
                throw new ArgumentNullException(nameof(procedure));

            if (procedure.Table == null)
                throw new ArgumentNullException(nameof(procedure.Table));

            if (procedure.Table.PrimaryKeyColumn == null)
                throw new Exception($"A primary key column is required for table '{procedure.Table.Name}' when creating procedure of type '{procedure.ProcedureType}'");
            
            var tableName = GetTableName(procedure.Table.Name);
            var columns = procedure.Table.Columns;

            var sb = new StringBuilder();
            
            sb.Append("CREATE PROCEDURE [")
                .Append(GetProcedureName(procedure.Name))
                .Append("]");

            if (columns.Count > 0)
            {
                sb.Append("(")
                    .Append(_newLine);

                var i = 0;
                foreach (var column in columns)
                {
                    sb
                        .Append("     ")
                        .Append("@")
                        .Append(column.NameNormalized)
                        .Append(" ")
                        .Append(column.DbTypeNormalized)
                        .Append(i < columns.Count - 1 ? "," : "")
                        .Append(_newLine);
                    i += 1;
                }

                sb.Append(") ");

            }

            sb.Append("AS")
                .Append(_newLine)
                .Append(_newLine)
                .Append("SET NOCOUNT ON ")
                .Append(_newLine)
                .Append(_newLine);

            sb.Append(GetProcedurePlaceHolderComment())
                .Append(_newLine)
                .Append(_newLine);

            sb.Append("DECLARE @unique_id ")
                .Append(procedure.Table.PrimaryKeyColumn.DbTypeNormalized)
                .Append(";")
                .Append(_newLine)
                .Append(_newLine);

            sb.Append("IF EXISTS (SELECT ")
                .Append(procedure.Table.PrimaryKeyColumn.Name)
                .Append(" FROM ")
                .Append(tableName)
                .Append(" WHERE ")
                .Append("(")
                .Append(procedure.Table.PrimaryKeyColumn.Name)
                .Append(" = ")
                .Append("@")
                .Append(procedure.Table.PrimaryKeyColumn.NameNormalized)
                .Append("))")
                .Append(_newLine);

            // update

            sb.Append("BEGIN")
                .Append(_newLine)
                .Append(_newLine);
            
            if (columns.Count > 0)
            {
                sb
                    .Append("   ")
                    .Append("UPDATE ")
                    .Append(tableName)
                    .Append(" SET ")
                    .Append(_newLine);
                var i = 0;
                foreach (var column in columns)
                {
                    if (!column.PrimaryKey)
                    {
                        sb
                            .Append("       ")
                            .Append(column.Name)
                            .Append(" = ")
                            .Append("@")
                            .Append(column.NameNormalized)
                            .Append(i < columns.Count - 1 ? "," : "")
                            .Append(_newLine);
                    }
                    i += 1;
                }

                sb
                    .Append("       ")
                    .Append("WHERE ")
                    .Append(procedure.Table.PrimaryKeyColumn.Name)
                    .Append(" = ")
                    .Append("@")
                    .Append(procedure.Table.PrimaryKeyColumn.NameNormalized)
                    .Append(_newLine);
            }

            sb
                .Append(_newLine)
                .Append("     ")
                .Append("SET @unique_id = @")
                .Append(procedure.Table.PrimaryKeyColumn.Name)
                .Append(";")
                .Append(_newLine)
                .Append(_newLine);

            sb
                .Append("END")
                .Append(_newLine)
                .Append("ELSE")
                .Append(_newLine)
                .Append("BEGIN")
                .Append(_newLine)
                .Append(_newLine);

            if (columns.Count > 0)
            {
                sb
                    .Append("   ")
                    .Append("INSERT INTO ")
                    .Append(tableName)
                    .Append(" ( ")
                    .Append(_newLine);
                var i = 0;
                foreach (var column in columns)
                {
                    if (!column.PrimaryKey)
                    {
                        sb
                            .Append("       ")
                            .Append(column.Name)
                            .Append(i < columns.Count - 1 ? "," : "")
                            .Append(_newLine);
                    }
                    i += 1;
                }

                sb
                    .Append("   ")
                    .Append(") VALUES (")
                    .Append(_newLine);
                i = 0;
                foreach (var column in columns)
                {
                    if (!column.PrimaryKey)
                    {
                        sb
                            .Append("       ")
                            .Append("@")
                            .Append(column.NameNormalized)
                            .Append(i < columns.Count - 1 ? "," : "")
                            .Append(_newLine);
                    }
                    i += 1;
                }

                sb
                    .Append("     ")
                    .Append(")");

                sb
                    .Append(_newLine)
                    .Append(_newLine)
                    .Append("     ")
                    .Append("SET @unique_id = SCOPE_IDENTITY();")
                    .Append(_newLine)
                    .Append(_newLine);

            }
            
            sb.Append("END")
                .Append(_newLine)
                .Append(_newLine)
                .Append("SELECT @unique_id;");
            
            return sb.ToString();

        }
        
        private string GetTableName(string tableName)
        {
            return !string.IsNullOrEmpty(_tablePrefix)
                ? _tablePrefix + tableName
                : tableName;
        }

        private string GetProcedureName(string procedureName)
        {
            return !string.IsNullOrEmpty(_tablePrefix)
                ? _tablePrefix + procedureName
                : procedureName;
        }

        private string DescribeTableColumn(
            SchemaColumn column)
        {
            var sb = new StringBuilder();
            sb.Append(column.Name).Append(" ").Append(column.DbTypeNormalized);

            if (column.PrimaryKey)
            {
                sb.Append(" IDENTITY(1,1)");
            }
            else
            {
                if (!string.IsNullOrEmpty(column.DefaultValueNormalizsed))
                    sb.Append(" DEFAULT (").Append(column.DefaultValueNormalizsed).Append(")");
            }
             
            sb.Append(column.Nullable ? " NULL" : " NOT NULL");
            return sb.ToString();
        }

        private string GetProcedurePlaceHolderComment()
        {
            var moduleName = !string.IsNullOrEmpty(this._options.ModuleName) ?
                this._options.ModuleName :
                "N/A";
            return $"/******{_newLine}Module: {moduleName}{_newLine}Version: {this._options.Version}{_newLine}This stored procedure was generated programmatically by Plato on {DateTime.Now}. Changes made by hand may be lost.{_newLine}******/";
        }

        #endregion


    }
}
