using HtmlTags.Conventions;

namespace StudentProgress.Web.Infrastructure
{
    public class TagConventions : HtmlConventionRegistry
    {
        public TagConventions()
        {
            Labels.Always.BuildBy<DefaultDisplayLabelBuilder>();
        }
    }
}