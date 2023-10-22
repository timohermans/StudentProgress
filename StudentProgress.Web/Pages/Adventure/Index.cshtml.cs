using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudentProgress.Web.Lib.Data;

namespace StudentProgress.Web.Pages.Adventure;

public class Index : PageModel
{
    private readonly WebContext _db;
    private readonly ILogger<Index> _logger;

    public required Models.Adventure Adventure { get; set; }

    public Index(WebContext db, ILogger<Index> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<IActionResult> OnGet(int id)
    {
        var adventure = await _db.Adventures
            .Include(a => a.People)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (adventure == null)
        {
            return NotFound();
        }

        Adventure = adventure;

        return Page();
    }
}