using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudentProgress.Core.Data;
using StudentProgress.Web.Lib.Extensions;

namespace StudentProgress.Web.Pages.QuestLine;

public class CreateModel(WebContext db, ILogger<CreateModel> logger) : PageModel
{
    public int AdventureId { get; set; }
    public int? PersonId { get; set; }
    [BindProperty][Required] public string? Name { get; set; }

    public void OnGet(int adventureId, int? personId)
    {
        logger.LogDebug("Quest line creation get");
        AdventureId = adventureId;
        PersonId = personId;
    }

    public async Task<IActionResult> OnPost(int adventureId, int? personId)
    {
        logger.LogDebug("Quest line creation post");
        OnGet(adventureId, personId);

        var adventure = await db.Adventures.FirstOrDefaultAsync(a => a.Id == adventureId);
        if (adventure == null)
        {
            logger.LogInformation("Trying to create a quest line of an adventure that doesn't exist({adventureId})", adventureId);
            return NotFound();
        }

        if (await db.QuestLines.AnyAsync(ql => ql.Adventure.Id == adventureId && ql.Name == Name))
        {
            ModelState.AddModelError(nameof(adventure.Name), "This questline already exists!");
        }

        logger.LogDebug("Is modelstate valid: {IsValid}", ModelState.IsValid);

        if (!ModelState.IsValid)
        {
            logger.LogDebug("Quest line is invalid:{ValidationMessage}", Environment.NewLine + string.Join(Environment.NewLine,
                ModelState.SelectMany(m =>
                    m.Value?.Errors.Select(e => $"{m.Key}: {e.ErrorMessage}") ?? Array.Empty<string>())));
            this.HtmxRetargetTo("#questLineForm", "outerHTML");
            return Page();
        }

        var orders = await db.QuestLines
            .Where(ql => ql.Adventure.Id == adventureId)
            .Select(ql => ql.Order)
            .ToListAsync();
        var nextOrder = orders.Count != 0 ? orders.Max() + 1 : 0;
        var questLine = new Core.Models.QuestLine
        {
            Name = Name!,
            Order = nextOrder,
            Adventure = adventure
        };

        await db.QuestLines.AddAsync(questLine);
        await db.SaveChangesAsync();
        logger.LogInformation("Created quest line {questLine}", questLine);

        return RedirectToPage("/Adventure/Index", new { id = adventureId, personId });
    }
}