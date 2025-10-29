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

    [HtmlTargetElement("*", Attributes = ClassPrefix + "*")]
    public class ConditionClassTagHelper : TagHelper
    {
        private const string ClassPrefix = "condition-class-";

        [HtmlAttributeName("class")] public string CssClass { get; set; }
        private IDictionary<string, bool> _classValues;

        [HtmlAttributeName("", DictionaryAttributePrefix = ClassPrefix)]
        public IDictionary<string, bool> ClassValues
        {
            get { return _classValues ??= new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase); }
            set { _classValues = value; }
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var items = _classValues.Where(e => e.Value).Select(e => e.Key).ToList();

            if (!string.IsNullOrEmpty(CssClass))
                items.Insert(0, CssClass);

            if (items.Count == 0)
                return;
            
            output.Attributes.Add("class", string.Join(" ", items.ToArray()));
        }
    }
}