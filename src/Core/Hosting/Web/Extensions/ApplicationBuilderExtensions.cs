using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PlatoCore.Localization.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using PlatoCore.Hosting.Web.Middleware;
using PlatoCore.Hosting.Web.Routing;
using PlatoCore.Modules;

namespace PlatoCore.Hosting.Web.Extensions
{

    public static class ApplicationBuilderExtensions
    {

        public static IApplicationBuilder UsePlato(
            this IApplicationBuilder app,
            IHostEnvironment env,
            ILoggerFactory logger)
        {

            env.ContentRootFileProvider = new CompositeFileProvider(
                new ModuleEmbeddedFileProvider(app.ApplicationServices),
                env.ContentRootFileProvider);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            // -----------------------------

            // Add the EndpointRoutingMiddleware
            app.UseRouting();

            // All middleware from here onwards know which endpoint will be invoked
            app.UseCors();

            // Execute the endpoint selected by the routing middleware
            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllers();

                //endpoints.MapControllerRoute(
                //    name: "default",
                //    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapRazorPages();
            });

            // -----------------------------

            // Add custom error handling for specific status codes
            // UseStatusCodePages should be called before any request
            // handling middleware in the pipeline (for example,
            // Static File middleware and MVC middleware).
            app.UseStatusCodePages(context =>
            {
                switch (context.HttpContext.Response.StatusCode)
                {
                    case 401:
                        context.HttpContext.Response.Redirect("/denied");
                        break;
                    case 404:
                        context.HttpContext.Response.Redirect("/moved");
                        break;
                }

                return Task.CompletedTask;
            });

            // Load static files
            app.UserPlatoStaticFiles();

            // Monitor changes to locale directories
            app.UsePlatoLocalization();

            // Add any IApplicationFeatureProvider 
            app.UseModularApplicationFeatureProvider();

            // Create services container for each shell
            app.UseMiddleware<PlatoContainerMiddleware>();

            // Create unique pipeline for each shell
            app.UseMiddleware<PlatoRouterMiddleware>();

            return app;

        }

        private static void UserPlatoStaticFiles(this IApplicationBuilder app)
        {

            // Get static file options 
            var options = app.ApplicationServices.GetRequiredService<IOptions<StaticFileOptions>>().Value;
            options.RequestPath = "";

            // Use different static file providers depending on our environment
            var env = app.ApplicationServices.GetRequiredService<IHostEnvironment>();
            if (env.IsDevelopment())
            {
                options.FileProvider = new CompositeFileProvider(
                    new ModuleEmbeddedStaticFileProvider(env, app.ApplicationServices));
            }
            else
            {
                options.FileProvider = new CompositeFileProvider(
                        new ModuleEmbeddedStaticFileProvider(env, app.ApplicationServices),
                        env.ContentRootFileProvider);
            }

            app.UseStaticFiles(options);

        }

        public static void UseModularApplicationFeatureProvider(this IApplicationBuilder app)
        {
            // adds ThemingViewsFeatureProvider application part
            var partManager = app.ApplicationServices.GetRequiredService<ApplicationPartManager>();
            var viewFeatureProvider = app.ApplicationServices.GetRequiredService<IApplicationFeatureProvider<ViewsFeature>>();
            partManager.FeatureProviders.Add(viewFeatureProvider);
        }

    }

}
