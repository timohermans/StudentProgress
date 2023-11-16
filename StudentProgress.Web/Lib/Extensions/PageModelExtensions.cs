using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentProgress.Core.Extensions;

namespace StudentProgress.Web.Lib.Extensions;

public class HtmxOptions
{
    public string? RetargetToElement { get; init; }
}

public static class PageModelExtensions
{
    public static bool HasHtmxTrigger(this HttpRequest request, string name)
    {
        return request.Headers.ContainsPair("HX-Trigger", name);
    }

    public static void DispatchHtmxEvent(this HttpResponse response, string name)
    {
        response.Headers.Append("HX-Trigger", name);
    }

    public static IActionResult SeeOther(this PageModel page, string pageName, object? values = null)
    {
        page.Response.Headers.Append("Location", page.Url.Page(pageName, values));
        return new StatusCodeResult(303);
    }

    public static void HtmxRetargetTo(this PageModel page, string targetElementId, string? swap = null)
    {
        page.Response.Headers.Append("HX-Retarget", targetElementId);

        if (swap != null)
        {
            page.Response.Headers.Append("HX-Reswap", swap);
        }
    }
}