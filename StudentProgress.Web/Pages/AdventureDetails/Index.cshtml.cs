using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudentProgress.Core.Data;
using StudentProgress.Core.Models;
using StudentProgress.Web.Lib.Constants;
using StudentProgress.Web.Lib.Extensions;

namespace StudentProgress.Web.Pages.AdventureDetails;

public class Index(WebContext db, ILogger<Index> logger) : PageModel
{
    public required Adventure Adventure { get; set; }
    public Person? Person { get; set; }
    public int? QuestId { get; set; }

    public async Task<IActionResult> OnGet(int id, int? personId)
    {
        var adventure = await db.Adventures
            .Include(a => a.People)
            .Include(a => a.QuestLines)
            .ThenInclude(ql => ql.Quests)
            .ThenInclude(q => q.Objectives)
            .AsSplitQuery()
            .FirstOrDefaultAsync(a => a.Id == id);


        // if (personId != null)
        // {
        //     HttpContext.Session.SetInt32(SessionKeys.PersonId, personId.Value);
        //     logger.LogDebug($"person selected: {personId}");
        //     Person = await db.People.FirstOrDefaultAsync(p => p.Id == personId);
        //     if (Person == null) return this.NotFoundToBody();
        //     HttpContext.Session.SetInt32("PersonId", Person.Id);
        // }

        if (adventure == null)
        {
            return NotFound();
        }

        EnsureProperSessionStateFor(adventure);

        Adventure = adventure;

        return Page();
    }

    public async Task<IActionResult> OnDeleteRemovePerson(int id, int personId)
    {
        var adventure = await db.Adventures
            .Include(a => a.People)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (adventure == null)
        {
            return NotFound();
        }

        adventure.People = adventure.People.Where(p => p.Id != personId).ToList();
        await db.SaveChangesAsync();

        HttpContext.Session.Remove(SessionKeys.PersonId);
        return this.SeeOther("Index", new { id });
    }

    private void EnsureProperSessionStateFor(Adventure adventure)
    {
        var questId = HttpContext.Session.GetInt32(SessionKeys.QuestId);
        if (questId != null && adventure.QuestLines.Any(ql => ql.Quests.Any(q => q.Id == questId)))
        {
            HttpContext.Session.Remove(SessionKeys.QuestId);
        }

        var personId = HttpContext.Session.GetInt32(SessionKeys.PersonId);
        if (personId != null && adventure.People.Any(p => p.Id == personId))
        {
            HttpContext.Session.Remove(SessionKeys.PersonId);
        }

    }
}