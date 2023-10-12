using System.Linq;
using System.Text.RegularExpressions;
using HtmlTags;
using HtmlTags.Conventions;
using HtmlTags.Conventions.Elements;
using HtmlTags.Conventions.Elements.Builders;

namespace StudentProgress.Web.Lib.Infrastructure
{
    public class DefaultDisplayLabelBuilder : IElementBuilder
    {
        public HtmlTag Build(ElementRequest request)
        {
            var fieldName = request.Accessor.PropertyNames.LastOrDefault();
            return new HtmlTag("label").Attr("for", DefaultIdBuilder.Build(request))
                .Text(BreakUpCamelCase(fieldName ?? ""));
        }

        private static string BreakUpCamelCase(string fieldName)
        {
            var patterns = new[]
            {
                "([a-z])([A-Z])",
                "([0-9])([a-zA-Z])",
                "([a-zA-Z])([0-9])"
            };
            var output = patterns.Aggregate(fieldName,
                static(current, pattern) =>
                    Regex.Replace(current, pattern, "$1 $2", RegexOptions.IgnorePatternWhitespace));
            return output.Replace('_', ' ');
        }
    }
}