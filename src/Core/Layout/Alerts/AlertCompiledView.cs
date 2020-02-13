using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using PlatoCore.Layout.Views.Abstractions;

namespace PlatoCore.Layout.Alerts
{
    public class AlertCompiledView : CompiledView
    {

        private readonly AlertInfo _alert;

        public AlertCompiledView(AlertInfo alert)
        {
            _alert = alert;
        }

        public override Task<IHtmlContent> InvokeAsync(ViewContext context)
        {
            if (_alert == null)
            {
                return Task.FromResult((IHtmlContent)HtmlString.Empty);
            }

            var builder = new HtmlContentBuilder();

            var htmlContentBuilder = builder
                .AppendHtml("<div class=\"alert alert-dismissible fade show")
                .AppendHtml(GetCssClass())
                .AppendHtml("\" role=\"alert\">")
                .AppendHtml(_alert.Message)
                .AppendHtml("<button type=\"button\" class=\"close\" data-dismiss=\"alert\" aria-label=\"Close\">")
                .AppendHtml("<span aria-hidden=\"true\">&times;</span>")
                .AppendHtml("</button>")
                .AppendHtml("</div>");

            return Task.FromResult((IHtmlContent)builder);

        }

        string GetCssClass()
        {
            switch (_alert.Type)
            {
                case AlertType.Success:
                    return " alert-success";
                case AlertType.Info:
                    return " alert-info";
                case AlertType.Warning:
                    return " alert-warning";
                case AlertType.Danger:
                    return " alert-danger";
            }

            return string.Empty;

        }

    }

}
