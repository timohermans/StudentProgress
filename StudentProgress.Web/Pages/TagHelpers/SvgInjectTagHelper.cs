using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace StudentProgress.Web.Pages.TagHelpers
{
    public class SvgInjectTagHelper : TagHelper
    {
        private readonly IWebHostEnvironment _env;

        public string Src { get; set; } = null!;

        public SvgInjectTagHelper(IWebHostEnvironment env) => _env = env;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "";
            
            var filePath = Path.Combine(_env.WebRootPath, Src);

            if (!File.Exists(filePath))
            {
                output.Content.SetHtmlContent("<b>svg-not-found</b>");
                return;
            }
            
            output.Content.SetHtmlContent(File.ReadAllText(filePath));
        }
    }
}