using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace StudentProgress.Web.Pages.TagHelpers;

public class SvgInjectTagHelper : TagHelper
{
    private readonly IWebHostEnvironment _env;

    public string Src { get; set; } = null!;
    public string? Class { get; set; }

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

        var svgString = File.ReadAllText(filePath) ?? throw new ArgumentNullException("svg still empty?");

        string svgPrefix = "<svg";
        int tagIndex = svgString.IndexOf("<svg", StringComparison.Ordinal);
        if (svgString.Contains("<svg") && Class != null)
        {
           svgString = svgString.Insert(tagIndex + svgPrefix.Length, $" class=\"{Class}\"");
        }
        
        
        output.Content.SetHtmlContent(svgString);
    }
}