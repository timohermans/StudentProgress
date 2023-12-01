using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentProgress.Core.Constants;
using StudentProgress.Core.Data;
using StudentProgress.Core.Models;
using StudentProgress.Web.Lib.Constants;
using StudentProgress.Web.Lib.Extensions;

namespace StudentProgress.Web.Pages.Objective;

public class Index(WebContext db) : PageModel
{
    [BindProperty] public int Id { get; set; }

    public async Task<IActionResult> OnPostProgress()
    {
        var personId = HttpContext.Session.GetInt32(SessionKeys.PersonId);

        var objective = await db.Objectives
            .Include(o => o.Progresses.Where(p => p.Person.Id == personId))
            .ThenInclude(p => p.Person)
            .FirstOrDefaultAsync(o => o.Id == Id);

        if (objective == null)
        {
            return this.NotFoundToBody();
        }

        if (!personId.HasValue)
        {
            ModelState.AddModelError(nameof(Core.Models.Objective.Id),
                "No person selected yet. Select a person first");
        }
        else if (!db.People.Exists(p => p.Id == personId))
        {
            ModelState.AddModelError(nameof(Core.Models.Objective.Id),
                "Person doesn't seem to exist. Choose another person");
        }
        else if (!db.Adventures.Any(a =>
                     a.QuestLines.Any(ql => ql.Quests.Any(q => q.Objectives.Any(o => o.Id == Id)))
                     && a.People.Any(p => p.Id == personId)))
        {
            ModelState.AddModelError(nameof(Core.Models.Objective.Id),
                "Person is not part of the adventure!");
        }

        if (objective.Progresses.Any(p => p.Person.Id == personId && p.AchievedAt.Date == DateTime.Today))
        {
            ModelState.AddModelError(nameof(Core.Models.Objective.Id),
                "You cannot add more than one progress per objective per day!");
        }

        if (!ModelState.IsValid)
        {
            return Partial("_Item", objective);
        }

        objective.Progresses.Add(new ObjectiveProgress
        {
            Person = await db.People.FirstAsync(p => p.Id == personId!.Value),
            AchievedAt = DateTime.Now
        });
        await db.SaveChangesAsync();

        Response.DispatchHtmxEvent(HtmxEvents.ProgressCreated);
        return Partial("_Item", objective);
    }
}