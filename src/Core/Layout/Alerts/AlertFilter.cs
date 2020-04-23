using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using PlatoCore.Net.Abstractions;

namespace PlatoCore.Layout.Alerts
{

    public class AlertFilter : IActionFilter, IAsyncResultFilter
    {

        private IList<AlertInfo> _alerts = new List<AlertInfo>();
        private const string _cookieName = "plato_alerts";      
        private bool _deleteCookie = false;

        private readonly ILayoutUpdater _layoutUpdater;
        private readonly ICookieBuilder _cookieBuilder;
        private readonly ILogger<AlertFilter> _logger;
        private readonly HtmlEncoder _htmlEncoder;
        private readonly IAlerter _alerter;

        public AlertFilter(
            ICookieBuilder cookieBuilder,
            ILayoutUpdater layoutUpdater,
            ILogger<AlertFilter> logger,
            HtmlEncoder htmlEncoder,
            IAlerter alerter)
        {
            _layoutUpdater = layoutUpdater;
            _cookieBuilder = cookieBuilder;
            _htmlEncoder = htmlEncoder;
            _alerter = alerter;       
            _logger = logger;
        }

        #region "Filter Implementation"

        public void OnActionExecuting(ActionExecutingContext context)
        {

            // 1.
            var json = _cookieBuilder.Contextulize(context.HttpContext).Get(_cookieName);

            if (String.IsNullOrEmpty(json))
            {
                return;
            }

            // Deserialize alerts store
            var alerts = DeserializeAlerts(json);
            if (alerts == null)
            {
                _deleteCookie = true;
                return;
            }

            if (alerts.Count == 0)
            {
                return;
            }

            // Ensure alerts are available for the entire request
            _alerts = alerts;

        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            
            // 2.
            var alerts = _alerter.Alerts();

            if (alerts == null)
            {
                return;
            }

            if (alerts.Count == 0 && _alerts.Count == 0)
            {
                return;
            }
            
            // Persist alerts for the entire request so they are available
            // for display within OnResultExecutionAsync below
            foreach (var alert in alerts)
            {
                _alerts.Add(alert);
            }
            
            // Result is not a view, so assume a redirect and assign values to persistence
            if (!(context.Result is ViewResult) && _alerts.Count > 0)
            {
                _cookieBuilder
                    .Contextulize(context.HttpContext)
                    .Append(
                        _cookieName,
                        SerializeAlerts(_alerts),
                        new CookieOptions
                        {
                            HttpOnly = true
                        });
            }

        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            
            // 3.
            if (_deleteCookie)
            {
                DeleteCookies(context);
                await next();
                return;
            }
            
            // We don't have any alerts
            if (_alerts.Count == 0)
            {
                await next();
                return;
            }

            // The controller action didn't return a view result so no need to continue execution
            if (!(context.Result is ViewResult result))
            {
                await next();
                return;
            }

            // Check early to ensure we are working with a LayoutViewModel
            if (!(result.Model is LayoutViewModel model))
            {
                await next();
                return;
            }

            // Add alerts to alerts zone within layout view model
            if (context.Controller is Controller controller)
            {
                var updater = await _layoutUpdater.GetLayoutAsync(controller);
                await updater.UpdateLayoutAsync(async layout =>
                {
                    layout.Alerts = await updater.UpdateZoneAsync(layout.Alerts, zone =>
                    {
                        foreach (var alert in _alerts)
                        {
                            zone.Add(new AlertCompiledView(alert));
                        }
                    });
                    return layout;
                });
            }
            
            // We've displayed our alert so delete persistence
            // to ensure no further alerts are displayed
            DeleteCookies(context);

            // Finally execute the controller result
            await next();

        }

        #endregion
        
        #region "Private Methods"

        IList<AlertInfo> DeserializeAlerts(string messages)
        {
         
            List<AlertInfo> alerts;
            try
            {
                alerts = JsonConvert.DeserializeObject<List<AlertInfo>>(messages, JsonSettings());
            }
            catch (Exception e)
            {
                // A problem occurring deserializing the alerts
                // Return null to ensure _deleteCookie is set to true
                // and persistence is deleted within OnResultExecutionAsync
                _logger.LogError(e, e.Message);
                alerts = null;
            }

            return alerts;

        }

        string SerializeAlerts(IList<AlertInfo> alert)
        {
            
            var output = string.Empty;
            try
            {
                output = JsonConvert.SerializeObject(alert, JsonSettings());
            }
            catch (Exception e)
            {
                // A problem occurring deserializing the alerts
                _logger.LogError(e, e.Message);
            }

            return output;

        }

        JsonSerializerSettings JsonSettings()
        {
            return new JsonSerializerSettings()
            {
                Converters = new List<JsonConverter>()
                {
                    new AlertInfoConverter(_htmlEncoder)
                }
            };
        }

        void DeleteCookies(ResultExecutingContext context)
        {
            _cookieBuilder
                .Contextulize(context.HttpContext)
                .Delete(_cookieName);
        }
        
        #endregion
        
    }
}
