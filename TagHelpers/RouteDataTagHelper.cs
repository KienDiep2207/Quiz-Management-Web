using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace DALTWNC_QUIZ.TagHelpers
{
    [HtmlTargetElement("div", Attributes = "route-data")]
    public class RouteDataTagHelper : TagHelper
    {
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; } = default!;

        [HtmlAttributeName("route-data")]
        public bool ShowRouteData { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (ShowRouteData)
            {
                var routeData = ViewContext.RouteData.Values;
                var content = new StringBuilder();

                content.Append("<h5 class='text-info'>Thông tin Định tuyến (Route Data)</h5>");
                content.Append("<dl class='row'>");

                foreach (var kvp in routeData)
                {
                    content.Append($"<dt class='col-sm-4 fw-bold'>{kvp.Key}:</dt>");
                    content.Append($"<dd class='col-sm-8'>{kvp.Value}</dd>");
                }

                content.Append("</dl>");

                output.Content.SetHtmlContent(content.ToString());
                string existingClasses = output.Attributes["class"]?.Value?.ToString() ?? string.Empty;
                output.Attributes.SetAttribute("class", "card p-3 bg-light mt-4 mb-4");
            }
        }
    }
}