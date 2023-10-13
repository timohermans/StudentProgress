using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StudentProgress.Web.Lib.Extensions;

public static class PageModelExtensions
{
    public static bool HasHtmxTrigger(this HttpRequest request, string name)
    {
        return request.Headers.ContainsPair("HX-Trigger", name);
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
        return triggerIdsForPartial.Any(id => page.Request.HasHtmxTrigger(id))
            ? page.Partial(partialName, page)
            : page.Page();
    }

    public static IActionResult SeeOther(this PageModel page, string location)
    {
        page.Response.Headers.Add("Location", location);
        return new StatusCodeResult(303);
    }
}