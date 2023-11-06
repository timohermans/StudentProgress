using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudentProgress.Web.Lib.Data;
using StudentProgress.Web.Lib.Extensions;
using StudentProgress.Web.Models;

namespace StudentProgress.Web.Pages.Adventure;

public class Index : PageModel
{
    private readonly WebContext _db;
    private readonly ILogger<Index> _logger;

    public Models.Adventure Adventure { get; set; }

    public Index(WebContext db, ILogger<Index> logger)
    {
        _db = db;
        _logger = logger;
    }

    public Person? Person { get; set; }

    public async Task<IActionResult> OnGet(int id, int? personId)
    {
        _logger.LogDebug($"person selected: {personId}");
        var adventure = await _db.Adventures
            .Include(a => a.People)
            .Include(a => a.QuestLines)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (personId != null)
        {
            Person = await _db.People.FirstOrDefaultAsync(p => p.Id == personId);
            if (Person == null) return NotFound();
        }

        if (adventure == null)
        {
            return NotFound();
        }

        Adventure = adventure;

        adventure.QuestLines.Add(new QuestLine
        {
            Name = "Empire of the client server web dev",
            Order = 1
        });
        adventure.QuestLines.Add(new QuestLine
        {
            Name = "Quality over quantity",
            Order = 2
        });

        return Page();
    }

    public async Task<IActionResult> OnDeleteRemovePerson(int id, int personId)
    {
        var adventure = await _db.Adventures
            .Include(a => a.People)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (adventure == null)
        {
            return NotFound();
        }

        adventure.People = adventure.People.Where(p => p.Id != personId).ToList();
        await _db.SaveChangesAsync();
        
        return this.SeeOther("Index", new { id });
    }
}