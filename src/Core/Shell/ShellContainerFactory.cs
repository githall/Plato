using System;
using System.Linq;
using PlatoCore.Abstractions;
using PlatoCore.Models.Shell;
using PlatoCore.Shell.Extensions;
using PlatoCore.Data.Abstractions;
using PlatoCore.Shell.Abstractions;
using PlatoCore.Abstractions.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace PlatoCore.Shell
{

    public class ShellContainerFactory : IShellContainerFactory
    {

        private readonly IServiceCollection _applicationServices;
        private readonly IServiceProvider _serviceProvider;

        public ShellContainerFactory(
            IServiceProvider serviceProvider,    
            IServiceCollection applicationServices)
        {
            _applicationServices = applicationServices;
            _serviceProvider = serviceProvider;        
        }

        public IServiceProvider CreateContainer(IShellSettings settings, ShellBlueprint blueprint)
        {

            // Clone services
            var tenantServiceCollection = _serviceProvider.CreateChildContainer(_applicationServices);

            // Add tenant specific settings
            tenantServiceCollection.AddSingleton(settings);
            tenantServiceCollection.AddSingleton(blueprint.Descriptor);
            tenantServiceCollection.AddSingleton(blueprint);

            // Add tenant specific data context options
            tenantServiceCollection.Configure<DbContextOptions>(options =>
            {
                options.ConnectionString = settings.ConnectionString;
                options.DatabaseProvider = settings.DatabaseProvider;
                options.TablePrefix = settings.TablePrefix;
            });

            // Register all tenant specific services
            tenantServiceCollection.AddPlatoTenant();

            // Add StartUps from modules defined in blueprint descriptor as services
            var moduleServiceCollection = _serviceProvider.CreateChildContainer(_applicationServices);

            // TODO: Add checks to ensure type is actually enabled as a feature within our shell descriptor
            foreach (var type in blueprint.Dependencies
                .Where(t => typeof(IStartup).IsAssignableFrom(t.Key)))
            {
                moduleServiceCollection.AddSingleton(typeof(IStartup), type.Key);
                tenantServiceCollection.AddSingleton(typeof(IStartup), type.Key);
            }

            // Add a default configuration if none has been provided
            var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();

            // Add default services
            moduleServiceCollection.TryAddSingleton(configuration);
            tenantServiceCollection.TryAddSingleton(configuration);

            // Make shell settings available to the modules
            moduleServiceCollection.AddSingleton(settings);
            moduleServiceCollection.AddSingleton(blueprint.Descriptor);
            moduleServiceCollection.AddSingleton(blueprint);

            // Configure module StartUps
            var moduleServiceProvider = moduleServiceCollection.BuildServiceProvider();
            var startups = moduleServiceProvider.GetServices<IStartup>();
            foreach (var startup in startups)
            {
                startup.ConfigureServices(tenantServiceCollection);
            }

            (moduleServiceProvider as IDisposable).Dispose();

            // We need to register IAuthenticationSchemeProvider again at the tenant level because 
            // it holds a reference to an underlying dictionary (_schemes) representing registered 
            // authentication schemes. These schemes can be distinct depending on the available modules 
            // within Plato so we register IAuthenticationSchemeProvider again as a singletone within 
            // the tenant services after module initialization.
            // https://github.com/aspnet/HttpAbstractions/blob/master/src/Microsoft.AspNetCore.Authentication.Core/AuthenticationSchemeProvider.cs
            tenantServiceCollection.AddSingleton<IAuthenticationSchemeProvider, AuthenticationSchemeProvider>();
            
            // Return our tenant service provider
            var shellServiceProvider = tenantServiceCollection.BuildServiceProvider();
            return shellServiceProvider;

        }

    }

}