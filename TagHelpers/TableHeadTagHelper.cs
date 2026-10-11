using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DALTWNC_QUIZ.TagHelpers
{
    [HtmlTargetElement("quiz-table-head")]
    public class TableHeadTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "thead";

            output.TagMode = TagMode.StartTagAndEndTag;

            output.Attributes.SetAttribute("class", "table-primary text-uppercase");
        }
    }
}