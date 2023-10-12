using Htmx;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StudentProgress.Web.Lib.Extensions;

public static class PageModelExtensions
{
    public static IActionResult PageOrPartial(this PageModel page, string partialName)
    {
        return page.Request.IsHtmx() && !page.Request.IsHtmxBoosted() ? page.Partial(partialName, page) : page.Page();
    }
}