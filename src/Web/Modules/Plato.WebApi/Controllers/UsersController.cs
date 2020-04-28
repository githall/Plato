using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Data.Abstractions;
using PlatoCore.Models.Users;
using PlatoCore.Stores.Abstractions.Users;
using PlatoCore.Stores.Users;
using Plato.WebApi.Models;
using PlatoCore.Hosting.Web.Abstractions;

namespace Plato.WebApi.Controllers
{

    public class UsersController : BaseWebApiController
    {

        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IContextFacade _contextFacade;

        public UsersController(
            IPlatoUserStore<User> platoUserStore,         
            IContextFacade contextFacade)
        {
            _platoUserStore = platoUserStore;
            _contextFacade = contextFacade;
        }

        #region "Actions"

        [HttpGet]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> Get(
            int page = 1,
            int size = 10,
            string keywords = "",
            string sort = "LastLoginDate",
            OrderBy order = OrderBy.Desc)
        {

            var users = await GetUsers(
                page,
                size,
                keywords,
                sort,
                order);
            
            IPagedResults<UserApiResult> results = null;
            if (users != null)
            {
                results = new PagedResults<UserApiResult>
                {
                    Total = users.Total
                };

                foreach (var user in users.Data)
                {

                    var profileUrl = _contextFacade.GetRouteUrl(new RouteValueDictionary()
                    {
                        ["area"] = "Plato.Users",
                        ["controller"] = "Home",
                        ["action"] = "Display",
                        ["opts.id"] = user.Id,
                        ["opts.alias"] = user.Alias
                    });

                    if (string.IsNullOrEmpty(user.Avatar.Url))
                    {                     
                        user.Avatar.Url = _contextFacade.GetRouteUrl(user.Avatar.DefaultRoute);
                    }

                    results.Data.Add(new UserApiResult()
                    {
                        Id = user.Id,
                        DisplayName = user.DisplayName,
                        UserName = user.UserName,
                        Url = profileUrl,
                        Avatar = user.Avatar
                    });
                }
            }

            IPagedApiResults<UserApiResult> output = null;
            if (results != null)
            {
                output = new PagedApiResults<UserApiResult>()
                {
                    Page = page,
                    Size = size,
                    Total = results.Total,
                    TotalPages = results.Total.ToSafeCeilingDivision(size),
                    Data = results.Data
                };
            }

            return output != null
                ? base.Result(output)
                : base.NoResults();

        }

        #endregion

        #region "Private Methods"

        async Task<IPagedResults<User>> GetUsers(
            int page,
            int pageSize,
            string username,
            string sortBy,
            OrderBy sortOrder)
        {

            return await _platoUserStore.QueryAsync()
                .Take(page, pageSize)
                .Select<UserQueryParams>(q =>
                {
                                        
                    q.HideSpam.True();
                    q.HideUnconfirmed.True();
                    q.HideBanned.True();

                    if (!String.IsNullOrEmpty(username))
                    {
                        q.Keywords.StartsWith(username);
                    }

                })
                .OrderBy(sortBy, sortOrder)
                .ToList();

        }

        #endregion

    }

}