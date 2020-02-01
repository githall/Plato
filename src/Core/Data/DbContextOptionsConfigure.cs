using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PlatoCore.Data.Abstractions;

namespace PlatoCore.Data
{

    public class DbContextOptionsConfigure : IConfigureOptions<DbContextOptions>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public DbContextOptionsConfigure()
        {
        }

        public DbContextOptionsConfigure(
            IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public void Configure(DbContextOptions options)
        {
            // default configuration
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var configuration = scope.ServiceProvider.GetRequiredService<IConfigurationRoot>();               
                options.ConnectionString = configuration.GetConnectionString("DefaultConnection");
                options.DatabaseProvider = "SqlClient";
                options.TablePrefix = "";

            }
        }

    }

}