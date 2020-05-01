﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PlatoCore.Text.Alias;
using PlatoCore.Text.Diff;
using PlatoCore.Text.UriExtractors;
using PlatoCore.Text.Abstractions;
using PlatoCore.Text.Abstractions.Diff;

namespace PlatoCore.Text.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoText(
            this IServiceCollection services)
        {

            services.TryAddSingleton<IAliasCreator, AliasCreator>();
            
            services.TryAddSingleton<IImageUriExtractor, ImageUriExtractor>();
            services.TryAddSingleton<IAnchorUriExtractor, AnchorUriExtractor>();            
            services.TryAddSingleton<IDefaultHtmlEncoder, DefaultHtmlEncoder>();
            services.TryAddSingleton<ITextParser, TextParser>();

            services.TryAddTransient<IPluralize, Pluralize>();
            services.TryAddTransient<IKeyGenerator, KeyGenerator>();

            services.TryAddSingleton<IDiffer, Differ>();
            services.TryAddSingleton<IInlineDiffBuilder, InlineDiffBuilder>();
            services.TryAddSingleton<ISideBySideDiffBuilder, SideBySideDiffBuilder>();
            
            return services;

        }

    }

}
