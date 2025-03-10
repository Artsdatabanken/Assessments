using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Assessments.Web.Infrastructure
{
    [HtmlTargetElement("*", Attributes = "is-visible")]
    public class VisibilityTagHelper : TagHelper
    {
        public bool IsVisible { get; set; } = true;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!IsVisible)
                output.SuppressOutput();

            base.Process(context, output);
        }

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (!IsVisible)
                output.SuppressOutput();

            return base.ProcessAsync(context, output);
        }
    }
}
