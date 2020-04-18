﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Hosting;
using PlatoCore.Models.Shell;
using Plato.SetUp.Services;
using PlatoCore.Hosting.Abstractions;

namespace Plato.SetUp
{

    public class Startup : StartupBase
    {

        private readonly IShellSettings _shellSettings;

        public Startup(IShellSettings shellSettings)
        {
            _shellSettings = shellSettings;
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ISetUpService, SetUpService>();
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            routes.MapAreaRoute(
                name: "SetUp",
                areaName: "Plato.SetUp",
                template: "",
                defaults: new { controller = "SetUp", action = "Index" }
            );

        }

    }

}