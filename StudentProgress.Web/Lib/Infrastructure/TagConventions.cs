using HtmlTags.Conventions;

namespace StudentProgress.Web.Lib.Infrastructure
{
    public class TagConventions : HtmlConventionRegistry
    {
        public TagConventions()
        {
            Labels.Always.BuildBy<DefaultDisplayLabelBuilder>();
        }
    }
}