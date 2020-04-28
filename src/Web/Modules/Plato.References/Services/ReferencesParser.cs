﻿using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Text.Abstractions;
using PlatoCore.Hosting.Web.Abstractions;

namespace Plato.References.Services
{
    public class ReferencesParser : IReferencesParser
    {

        private readonly IEntityStore<Entity> _entityStore;
        private readonly IHashTokenizer _hashTokenizer;
        private readonly ILinkTokenizer _linkTokenizer;
        private readonly IContextFacade _contextFacade;    

        public ReferencesParser(        
            IEntityStore<Entity> entityStore,
            IHashTokenizer hashTokenizer,
            IContextFacade contextFacade,
            ILinkTokenizer linkTokenizer)
        {
            _contextFacade = contextFacade;
            _linkTokenizer = linkTokenizer;
            _entityStore = entityStore;
            _hashTokenizer = hashTokenizer;           
        }

        public async Task<string> ParseAsync(string input)
        {

            // First parse more explicit link hash references i.e. #123(optional text)
            input = await ParseLinkTokensAsync(input);

            // Next parse broader simpler #123 references
            return await ParseHashTokensAsync(input);

        }

        public async Task<IEnumerable<Entity>> GetEntitiesAsync(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            var tokens = _hashTokenizer.Tokenize(input);
            if (tokens != null)
            {
                return await GetEntitiesAsync(tokens);
            }

            return null;
        }

        // ------------

        async Task<string> ParseLinkTokensAsync(string input)
        {


            // We need input to parse
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            // Build tokens
            var tokens = _linkTokenizer.Tokenize(input);

            // Ensure we have tokens to parse
            if (tokens == null)
            {
                return input;
            }

            // Prevent multiple enumeration
            var tokenList = tokens.ToList();

            // Get all referenced entities
            var entities = await GetEntitiesAsync(tokenList);
            if (entities != null)
            {

                var entityList = entities.ToList();
                var sb = new StringBuilder();

                var insideHtmlTag = false;
                var hasLinkText = false;

                for (var i = 0; i < input.Length; i++)
                {

                    if (input[i] == '<') { insideHtmlTag = true; }
                    if (input[i] == '>') { insideHtmlTag = false; }
                    
                    foreach (var token in tokenList)
                    {
                        // Token start
                        if (i == token.Start)
                        {
                            var entity = entityList.FirstOrDefault(e =>
                                e.Id.ToString().Equals(token.Value, StringComparison.Ordinal));
                            if (entity != null)
                            {
                                var url = _contextFacade.GetRouteUrl(new RouteValueDictionary()
                                {
                                    ["area"] = entity.ModuleId,
                                    ["controller"] = "Home",
                                    ["action"] = "Display",
                                    ["opts.id"] = entity.Id,
                                    ["opts.alias"] = entity.Alias
                                });
                                var popperUrl = _contextFacade.GetRouteUrl(new RouteValueDictionary()
                                {
                                    ["area"] = "Plato.Entities",
                                    ["controller"] = "Home",
                                    ["action"] = "GetEntity",
                                    ["opts.id"] = entity.Id,
                                    ["opts.alias"] = entity.Alias
                                });
                                if (!insideHtmlTag)
                                {
                                    sb.Append("<a href=\"")
                                        .Append(url)
                                        .Append("\" ")
                                        .Append("data-provide=\"popper\" ")
                                        .Append("data-popper-url=\"")
                                        .Append(popperUrl)
                                        .Append("\" class=\"reference-link\">");
                                    if (!string.IsNullOrEmpty(token.Text))
                                    {
                                        sb.Append(token.Text);
                                        hasLinkText = true;
                                    }
                                }
                            }
                        }
                    }

                    if (!hasLinkText)
                    {
                        sb.Append(input[i]);
                    }

                    foreach (var token in tokenList)
                    {
                        if (i == token.End)
                        {
                            var entity = entityList.FirstOrDefault(e =>
                                e.Id.ToString().Equals(token.Value, StringComparison.Ordinal));
                            if (entity != null)
                            {
                                if (!insideHtmlTag)
                                {
                                    sb.Append("</a>");
                                }
                            }
                            hasLinkText = false;
                        }

                    }

                }

                return sb.ToString();
            }

            return input;

        }

        async Task<string> ParseHashTokensAsync(string input)
        {


            // We need input to parse
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            // Build tokens
            var tokens = _hashTokenizer.Tokenize(input);

            // Ensure we have tokens to parse
            if (tokens == null)
            {
                return input;
            }

            // Prevent multiple enumeration
            var tokenList = tokens.ToList();

            // Get all referenced entities
            var entities = await GetEntitiesAsync(tokenList);
            if (entities != null)
            {

                var entityList = entities.ToList();
                var sb = new StringBuilder();

                var insideHtmlTag = false;

                for (var i = 0; i < input.Length; i++)
                {
                    if (input[i] == '<')
                    {
                        insideHtmlTag = true;
                    }

                    if (input[i] == '>')
                    {
                        insideHtmlTag = false;
                    }

                    foreach (var token in tokenList)
                    {
                        // Token start
                        if (i == token.Start)
                        {

                            var entity = entityList.FirstOrDefault(e =>
                                e.Id.ToString().Equals(token.Value, StringComparison.Ordinal));
                            if (entity != null)
                            {
                                var url = _contextFacade.GetRouteUrl(new RouteValueDictionary()
                                {
                                    ["area"] = entity.ModuleId,
                                    ["controller"] = "Home",
                                    ["action"] = "Display",
                                    ["opts.id"] = entity.Id,
                                    ["opts.alias"] = entity.Alias
                                });
                                var popperUrl = _contextFacade.GetRouteUrl(new RouteValueDictionary()
                                {
                                    ["area"] = "Plato.Entities",
                                    ["controller"] = "Home",
                                    ["action"] = "GetEntity",
                                    ["opts.id"] = entity.Id,
                                    ["opts.alias"] = entity.Alias
                                });
                                if (!insideHtmlTag)
                                {
                                    sb.Append("<a href=\"")
                                        .Append(url)
                                        .Append("\" ")
                                        .Append("data-provide=\"popper\" ")
                                        .Append("data-popper-url=\"")
                                        .Append(popperUrl)
                                        .Append("\" class=\"reference-link\">");
                                }
                           
                            }
                        }
                    }

                    sb.Append(input[i]);

                    foreach (var token in tokenList)
                    {
                        if (i == token.End)
                        {
                            var entity = entityList.FirstOrDefault(e =>
                                e.Id.ToString().Equals(token.Value, StringComparison.Ordinal));
                            if (entity != null)
                            {
                                if (!insideHtmlTag)
                                {
                                    sb.Append("</a>");
                                }
                            }
                        }

                    }

                }

                return sb.ToString();
            }

            return input;

        }

        async Task<IEnumerable<Entity>> GetEntitiesAsync(IEnumerable<IToken> tokens)
        {

            var entityIds = GetDistinctTokenValues(tokens);
            if (entityIds?.Length > 0)
            {
                return await GetEntitiesByIdsAsync(entityIds.ToArray());
            }

            return null;

        }

        async Task<IEnumerable<Entity>> GetEntitiesByIdsAsync(string[] entityIds)
        {
            
            var entities = await _entityStore.QueryAsync()
                .Select<EntityQueryParams>(q =>
                {
                    q.Id.IsIn(entityIds.ToIntArray());
                })
                .ToList();

            return entities?.Data;

        }

        string[] GetDistinctTokenValues(IEnumerable<IToken> tokens)
        {
            var output = new List<string>();
            foreach (var token in tokens)
            {
                if (!output.Contains(token.Value))
                    output.Add(token.Value);
            }
            return output?.ToArray();
        }

    }

}
