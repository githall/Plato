﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Plato.Tenants.SignUp.Stores;
using PlatoCore.Abstractions;
using PlatoCore.Hosting.Web.Abstractions;
using PlatoCore.Text.Abstractions;
using PlatoCore.Abstractions.Extensions;

namespace Plato.Tenants.SignUp.Services
{

    public class SignUpValidator : ISignUpValidator
    {

        private readonly IActionDescriptorCollectionProvider _provider;        
        private readonly ISignUpStore<Models.SignUp> _signUpStore;
        private readonly IContextFacade _contextFacade;
        private readonly IAliasCreator _aliasCrator;

        public SignUpValidator(            
            IActionDescriptorCollectionProvider provider,
            ISignUpStore<Models.SignUp> signUpStore,
            IContextFacade contextFacade,
            IAliasCreator aliasCrator)
        {
            _contextFacade = contextFacade;            
            _signUpStore = signUpStore;
            _aliasCrator = aliasCrator;
            _provider = provider;
        }

        public Task<ICommandResultBase> ValidateEmailAsync(string email)
        {

            // TODO: Implement email validation

            var result = new CommandResultBase();
            return Task.FromResult((ICommandResultBase)result.Success());

        }

        public async Task<ICommandResultBase> ValidateCompanyNameAsync(string companyName)
        {

            var result = new CommandResultBase();
            var error = "The company name is invalid or is already in use. Please try a different company name.";

            // We always need a company name
            if (string.IsNullOrEmpty(companyName))
            {
                return result.Failed("A company name is required!");
            }

            // Validate characters
            var valid = true;
            foreach (var c in companyName)
            {
                // Not a letter, digit or space
                if (!c.IsLetterDigitOrSpace())
                {
                    valid = false;
                    break;
                }
            }

            // Only allow letters digits and spaces
            if (valid == false)
            {
                return result.Failed("The company name cannot contain special characters. Only letters, numbers and spaces are allowed. Please try a different company name.");
            }

            // Create the company name alias to compare with our blacklist
            var companyNameAlias = _aliasCrator.Create(companyName);

            // We need an alias
            if (string.IsNullOrEmpty(companyNameAlias))
            {
                return result.Failed(error);
            }

            // Does the company name appear in the blacklist?
            var blackList = GetBlackListedCompanyNames();
            foreach (var item in blackList)
            {
                if (item.Equals(companyName, StringComparison.OrdinalIgnoreCase))
                {
                    return result.Failed(error);
                }
                if (item.Equals(companyNameAlias, StringComparison.OrdinalIgnoreCase))
                {
                    return result.Failed(error);
                }
            }

            // ---------------
            // Does the company name already exist?
            // We need company names to be unique as the 
            // tenant RequestedPrefixUrl uses CompanyNameAlias
            // ---------------

            var signUps = await _signUpStore.QueryAsync()
                .Select<SignUpQueryParams>(q =>
                {
                    q.CompanyName.Or().Equals(companyName);
                    q.CompanyNameAlias.Or().Equals(companyName);
                })
                .ToList();

            if (signUps?.Data != null)
            {
                return result.Failed(error);
            }

            return result.Success();

        }

        private IList<string> GetBlackListedCompanyNames()
        {

            // Don't allow default tenant name
            var output = new List<string>()
            {
                "Default"
            };

            // Get all routes
            var routeValues = _provider.ActionDescriptors.Items.Select(x => new
            {
                Area = x.RouteValues["Area"],
                Controller = x.RouteValues["Controller"],
                Action = x.RouteValues["Action"]
            }).ToList();

            // Add all application routes to blacklist
            foreach (var routeValue in routeValues)
            {

                // Resolve the route values to the URL template
                var url = _contextFacade.GetRouteUrl(new Microsoft.AspNetCore.Routing.RouteValueDictionary()
                {
                    ["area"] = routeValue.Area,
                    ["controller"] = routeValue.Controller,
                    ["action"] = routeValue.Action,
                });

                // Ensure we could resolve the URL
                if (!string.IsNullOrEmpty(url))
                {
                    // Trim any / suffix
                    if (url.StartsWith("/"))
                    {
                        url = url.Substring(1, url.Length - 1);
                    }
                    output.Add(url);
                }       

            }

            return output;

        }

    }

}
