using System;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentProgress.Web.Lib.CanvasApi;
using StudentProgress.Web.Lib.Data;
using StudentProgress.Web.Lib.Extensions;
using StudentProgress.Web.Models;

namespace StudentProgress.Web.Pages.StudentGroups;

public class CreateModel : PageModel
{
    private readonly WebContext _db;
    private readonly ICanvasApiConfig _canvasConfig;

    [BindProperty] public Adventure Adventure { get; set; } = default!;

    public CreateModel(WebContext db, ICanvasApiConfig canvasConfig)
    {
        _db = db;
        _canvasConfig = canvasConfig;
    }

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

        await _db.Adventures.AddAsync(Adventure, token);
        await _db.SaveChangesAsync(token);

        Response.DispatchHtmxEvent("adventure-created");
        return Partial("_Actions", await _canvasConfig.CanUseCanvasApiAsync());
    }
}