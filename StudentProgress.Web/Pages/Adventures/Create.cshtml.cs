using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentProgress.Core.CanvasApi;
using StudentProgress.Core.Data;
using StudentProgress.Web.Lib.Extensions;

namespace StudentProgress.Web.Pages.Adventures;

public class CreateModel(WebContext db, ICanvasApiConfig canvasConfig) : PageModel
{
    [BindProperty] public Core.Models.Adventure Adventure { get; set; } = default!;

    public IActionResult OnGet()
    {
        Adventure = new()
        {
            DateStart = DateTime.Today,
            Name = ""
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken token)
    {
        if (!ModelState.IsValid)
        {
            this.HtmxRetargetTo("#actions");
            return Page();
        }

        await db.Adventures.AddAsync(Adventure, token);
        await db.SaveChangesAsync(token);

        Response.DispatchHtmxEvent("adventure-created");
        return Partial("_Actions", canvasConfig.CanUseCanvasApiAsync());
    }
}