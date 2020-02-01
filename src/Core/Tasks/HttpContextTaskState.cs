using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PlatoCore.Tasks.Abstractions;

namespace PlatoCore.Tasks
{
    public class HttpContextTaskState : IDeferredTaskState
    {

        private readonly HttpContext _httpContext;

        public HttpContextTaskState(IHttpContextAccessor httpContextAccessor)
        {
            _httpContext = httpContextAccessor.HttpContext;
        }

        public IList<Func<DeferredTaskContext, Task>> Tasks
        {
            get
            {
                if (!_httpContext.Items.TryGetValue(typeof(HttpContextTaskState), out var tasks))
                {
                    tasks = new List<Func<DeferredTaskContext, Task>>();
                    _httpContext.Items[typeof(HttpContextTaskState)] = tasks;
                }
                return (IList<Func<DeferredTaskContext, Task>>)tasks;
            }
        }

    }

}
