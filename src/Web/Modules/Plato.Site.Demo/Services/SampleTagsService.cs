using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using PlatoCore.Abstractions;
using PlatoCore.Data.Abstractions;
using PlatoCore.Features.Abstractions;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Models.Features;
using Plato.Site.Demo.Models;
using Plato.Tags.Models;
using Plato.Tags.Services;

namespace Plato.Site.Demo.Services
{

    public class SampleTagsService : ISampleTagsService
    {
      
        public string[] ExampleTags => new string[]
        {
            "percent-encoding",
            "udf",
            "spark-sql",
            "sql",
            "sql-server",
            "uriencoding",
            "slicing",
            "array-slice",
            "data-uri-scheme",
            "watir-webdriver",
            "azure-app-service",
            "icpc",
            "intel-c++",
            "c++",
            "intel",
            ".net-4.5.2",
            ".net-core",
            "ef6",
            "efcore",
            "ptr",
            "mysql-server",
            "mysql",
            "windows-server-2019",
            "php-mysqli",
            "intellij",
            "performance-tuning",
            "bug",
            "websockets",
            "xhr",
            "codecs",
            "cookies",
            "arithmetic",
            "plt-scheme",
            "drscheme",
            "msi",
            "vscode",
            "enumulation",
            "tuning",
            "performance",
            "sparse-array",
            "webflow",
            "android",
            "android-p",
            "vmware",
            "asp.net",
            "powershell",
            "app-store",
            "visual-studio",
            ".net-core-2.2",
            ".net-core-3.0",
            "mvc6",
            "jQuery",
            "bootstrap-4",
            "asset",
            "flickr-api",
            "orthogonality",
            "imports",
            "array-of-objects",
            "webworkers",
            "graphics-card",
            "sitemaps",
            "workspaces",
            "chart",
            "input-validation",
            "charting-controls",
            "csrf",
            "https",
            "tls",
            "karma",
            "php",
            "cs5",
            "locals",
            "locale",
            "localization",
            "elastic",
            "elastic-search",
            "web-browser",
            "browser",
            "java",
            "javascript",
            "ef-core",
            "lookahead",
            "lookbehind",
            "ios",
            "markdown",
            "html",
            "css",
            "includes",
            "client-authentication",
            "user-authentication",
            "generic-class",
            "generics",
            "c#",
            "c#-8",
            "c#-7",
            "postfix",
            "grunt",
            "glup",
            "bower",
            "vue",
            "angular",
            "oop",
            "functional",
            "google-api",
            "web-api",
            "slack-api",
            "sql-join",
            "sql-select",
            "io.js",
            "push-notifications",
            "php5",
            "swift",
            "phython",
            "ruby",
            "facebook-jssdk",
            "wcf",
            "swagga",
            "angularjs-ui-router",
            "ui-router",
            "void-pointer",
            "iphone-sdk",
            "like-operator",
            "where-operator",
            "query-execution-plans",
            "query-plan",
            "execution-plan",
            "css-box-model",
            "xcode8.3",
            "jquery-droppable",
            "jquery-validation",
            "jquery-autocomplete",
            "erlang",
            "enumerations",
            "decimals",
            "uml-modeling",
            ".net-4.5.1",
            "dotnetcore",
            "sso",
            "seo",
            "aws-s3",
            "azure",
            "app-service",
            "iis-8",
            "iis-10",
            "fulltext-searching",
            "fulltext",
            "breadcrumb",
            "paas",
            "saas",
            "reportingservices",
            "sql-reporting",
            "globalisation",
            "optimisation",
            "optimize",
            "initialize",
            "parser",
            "htaccess",
            "ttf",
            ".sql",
            "recursive",
            "branches",
            "git",
            "macro",
            "namespace",
            "resource",
            "transaction",
            "protocol",
            "theme",
            "notification",
            "container",
            "docker",
            "warning",
            "error",
            "permission",
            "annotation",
            "web-service",
            "web-application",
            "windows-service",
            "data-structure",
            "orderby",
            "jenkins",
            "async",
            "await",
            "code-audit",
            "refactor",
            "assembler",
            "repositories",
            "domain-specific-languages",
            "javascript-execution",
            "interoperability",
            "patches",
            "modules",
            "bash-script",
            "terminal",
            "shell",
            "console",
            "sql-driver",
            "odbc",
            "computer-graphics",
            "apache",
            "aspect-oriented",
            "dsl",
            "cache",
            "logger",
            "logging",
            "csproj",
            "mssql2019",
            "cronjob",
            "task-scheduler",
            "deferred-tasks",
            "task-scheduling",
            "auto-token",
            "j2ee",
            "vs2019",
            "svg",
            "webgl",
            "graph-ql",
            "oauth",
            "sql-query"
        };

        private readonly List<SampleDataDescriptor> Descriptors = new List<SampleDataDescriptor>()
        {
            new SampleDataDescriptor()
            {
                ModuleId = "Plato.Discuss",
                EntityType = "topic"
            },
            new SampleDataDescriptor()
            {
                ModuleId = "Plato.Docs",
                EntityType = "doc",
            },
            new SampleDataDescriptor()
            {
                ModuleId = "Plato.Articles",
                EntityType = "articles",
            },
            new SampleDataDescriptor()
            {
                ModuleId = "Plato.Ideas",
                EntityType = "idea"
            },
            new SampleDataDescriptor()
            {
                ModuleId = "Plato.Issues",
                EntityType = "issue"
            },
            new SampleDataDescriptor()
            {
                ModuleId = "Plato.Questions",
                EntityType = "question"
            }
        };

        private readonly ITagManager<TagBase> _tagManager;
        private readonly IContextFacade _contextFacade;
        private readonly IFeatureFacade _featureFacade;        
        private readonly IDbHelper _dbHelper;

        public SampleTagsService(
            ITagManager<TagBase> tagManager,            
            IContextFacade contextFacade,
            IFeatureFacade featureFacade,
            IDbHelper dbHelper)
        {            
            _featureFacade = featureFacade;
            _contextFacade = contextFacade;            
            _tagManager = tagManager;
            _dbHelper = dbHelper;
        }

        public async Task<ICommandResultBase> InstallAsync()
        {

            var output = new CommandResultBase();
            foreach (var descriptor in Descriptors)
            {

                var feature = await _featureFacade.GetFeatureByIdAsync(descriptor.ModuleId);
                if (feature == null)
                {
                    continue;
                }

                var result = await InstallCategoriesAsync(feature);
                if (!result.Succeeded)
                {
                    return output.Failed(result.Errors.ToArray());
                }

            }

            return output.Success();

        }

        public async Task<ICommandResultBase> UninstallAsync()
        {

            var output = new CommandResultBase();
            foreach (var descriptor in Descriptors)
            {

                // Ensure the feature is enabled
                var feature = await _featureFacade.GetFeatureByIdAsync(descriptor.ModuleId);
                if (feature == null)
                {
                    continue;
                }

                var result = await UninstallInternalAsync(feature);
                if (!result.Succeeded)
                {
                    return output.Failed(result.Errors.ToArray());
                }

            }

            return output.Success();

        }

        // ----------

        async Task<ICommandResultBase> InstallCategoriesAsync(IShellFeature feature)
        {

            // Validate

            if (feature == null)
            {
                throw new ArgumentNullException(nameof(feature));
            }

            if (string.IsNullOrEmpty(feature.ModuleId))
            {
                throw new ArgumentNullException(nameof(feature.ModuleId));
            }

            // Our result
            var result = new CommandResultBase();
                        
            foreach (var tag in ExampleTags)
            {
                var categoryResult = await InstallInternalAsync(feature, tag);
                if (!categoryResult.Succeeded)
                {
                    return result.Failed(categoryResult.Errors.ToArray());
                }
            }

            return result.Success();

        }

        async Task<ICommandResultBase> InstallInternalAsync(IShellFeature feature, string tag)
        {

            // Validate

            if (feature == null)
            {
                throw new ArgumentNullException(nameof(feature));
            }

            if (string.IsNullOrEmpty(feature.ModuleId))
            {
                throw new ArgumentNullException(nameof(feature.ModuleId));
            }

            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Our result
            var result = new CommandResultBase();

            var categoryResult = await _tagManager.CreateAsync(new TagBase()
            {
                FeatureId = feature.Id,           
                Name = tag,
                Description = $"An example desccription for the '{tag}' tag within '{feature.ModuleId}'.",            
                CreatedUserId = user?.Id ?? 0,
                CreatedDate = DateTimeOffset.UtcNow
            });

            if (!categoryResult.Succeeded)
            {
                return result.Failed(result.Errors.ToArray());
            }

            return result.Success();

        }

        async Task<ICommandResultBase> UninstallInternalAsync(IShellFeature feature)
        {

            // Validate

            if (feature == null)
            {
                throw new ArgumentNullException(nameof(feature));
            }

            if (string.IsNullOrEmpty(feature.ModuleId))
            {
                throw new ArgumentNullException(nameof(feature.ModuleId));
            }

            // Our result
            var result = new CommandResultBase();

            // Replacements for SQL script
            var replacements = new Dictionary<string, string>()
            {
                ["{featureId}"] = feature.Id.ToString()
            };

            // Sql to execute
            var sql = @"
                DELETE FROM {prefix}_EntityTags WHERE TagId IN (
                    SELECT Id FROM {prefix}_Tags WHERE FeatureId = {featureId}
                );             
                DELETE FROM {prefix}_Tags WHERE FeatureId = {featureId};
            ";

            // Execute and return result
            var error = string.Empty;
            try
            {
                await _dbHelper.ExecuteScalarAsync<int>(sql, replacements);
            }
            catch (Exception e)
            {
                error = e.Message;
            }

            return !string.IsNullOrEmpty(error)
                ? result.Failed(error)
                : result.Success();

        }

    }

}
