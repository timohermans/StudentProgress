using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
        response.Headers.Add("HX-Trigger", name);
    }

    /// <summary>
    /// Returns either a page or partial, based on the element ID in the HX-Trigger header.
    /// </summary>
    /// <param name="page">the current PageModel</param>
    /// <param name="partialName">the partial to return if it is requested</param>
    /// <param name="triggerIdForPartial">the wanted element ID that goes along with the wanted partial</param>
    /// <returns>IActionResult Page or Partial</returns>
    public static IActionResult PageOrPartial(this PageModel page, string partialName,
        params string[] triggerIdsForPartial)
    {
        return page.PageOrPartial(partialName, null, triggerIdsForPartial);
    }

    /// <summary>
    /// Returns either a page or partial, based on the element ID in the HX-Trigger header.
    /// </summary>
    /// <param name="page">the current PageModel</param>
    /// <param name="partialName">the partial to return if it is requested</param>
    /// <param name="triggerIdForPartial">the wanted element ID that goes along with the wanted partial</param>
    /// <returns>IActionResult Page or Partial</returns>
    public static IActionResult PageOrPartial(this PageModel page, string partialName, HtmxOptions? htmxOptions = null,
        params string[] triggerIdsForPartial)
    {
        if (!triggerIdsForPartial.Any(id => page.Request.HasHtmxTrigger(id)))
        {
            return page.Page();
        }

        if (htmxOptions?.RetargetToElement != null)
        {
            page.Response.Headers.Add("HX-Retarget", htmxOptions.RetargetToElement);
        }

        return page.Partial(partialName, page);
    }

    public static IActionResult SeeOther(this PageModel page, string pageName, object? values = null)
    {
        page.Response.Headers.Add("Location", page.Url.Page(pageName, values));
        return new StatusCodeResult(303);
    }

    public static void HtmxRetargetTo(this PageModel page, string targetElementId, string? swap = null)
    {
        page.Response.Headers.Add("HX-Retarget", targetElementId);

        if (swap != null)
        {
            page.Response.Headers.Add("HX-Reswap", swap);
        }
    }
}