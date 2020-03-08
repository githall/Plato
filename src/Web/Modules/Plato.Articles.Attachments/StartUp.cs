﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Models.Shell;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.ViewProviders.Abstractions;
using Plato.Articles.Models;
using PlatoCore.Layout.ViewProviders;
using Plato.Articles.Attachments.ViewProviders;
using Plato.Articles.Attachments.Navigation;
using PlatoCore.Navigation.Abstractions;

namespace Plato.Articles.Attachments
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

            // Register navigation provider     
            services.AddScoped<INavigationProvider, ArticleFooterMenu>();
          
            // View providers
            services.AddScoped<IViewProviderManager<Article>, ViewProviderManager<Article>>();
            services.AddScoped<IViewProvider<Article>, ArticleViewProvider>();
            services.AddScoped<IViewProviderManager<Comment>, ViewProviderManager<Comment>>();
            services.AddScoped<IViewProvider<Comment>, CommentViewProvider>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {

            // Download
            routes.MapAreaRoute(
                name: "ArticlesAttachmentDownload",
                areaName: "Plato.Articles.Attachments",
                template: "articles/attachments/download/{id:int}",
                defaults: new { controller = "Attachment", action = "Download" }
            );

            // Delete
            routes.MapAreaRoute(
                name: "ArticlesAttachmentDelete",
                areaName: "Plato.Articles.Attachments",
                template: "articles/attachments/delete/{id:int}",
                defaults: new { controller = "Attachment", action = "Delete" }
            );

        }

    }

}