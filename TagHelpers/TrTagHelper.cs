using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;
using System.Linq;

namespace DALTWNC_QUIZ.TagHelpers
{
    [HtmlTargetElement("tr", Attributes = "highlight")]
    [HtmlTargetElement("tr", Attributes = "is-active")]
    public class TrTagHelper : TagHelper
    {
        public bool Highlight { get; set; } = false;
        public bool IsActive { get; set; } = false;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var classBuilder = new StringBuilder();

            if (IsActive)
            {
                classBuilder.Append(" table-success ");
            }
            else
            {
                classBuilder.Append(" table-secondary ");
            }

            if (Highlight)
            {
                classBuilder.Append(" fw-bold ");
            }

            string existingClasses = output.Attributes["class"]?.Value?.ToString() ?? string.Empty;

            output.Attributes.SetAttribute("class", (existingClasses + classBuilder.ToString()).Trim());
        }
    }
}