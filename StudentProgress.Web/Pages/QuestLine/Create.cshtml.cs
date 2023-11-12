using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudentProgress.Web.Lib.Data;
using StudentProgress.Web.Lib.Extensions;

namespace StudentProgress.Web.Pages.QuestLine;

public class CreateModel : PageModel
{
    private readonly WebContext _db;
    private readonly ILogger<CreateModel> _logger;

    public int AdventureId { get; set; }
    public int? PersonId { get; set; }
    [BindProperty] [Required] public string? Name { get; set; }

    public CreateModel(WebContext db, ILogger<CreateModel> logger)
    {
        _db = db;
        _logger = logger;
    }

    public void OnGet(int adventureId, int? personId)
    {
        _logger.LogDebug("Quest line creation get");
        AdventureId = adventureId;
        PersonId = personId;
    }

    public async Task<IActionResult> OnPost(int adventureId, int? personId)
    {
        _logger.LogDebug("Quest line creation post");
        OnGet(adventureId, personId);

        var adventure = await _db.Adventures.FirstOrDefaultAsync(a => a.Id == adventureId);
        if (adventure == null)
        {
            _logger.LogInformation($"Trying to create a quest line of an adventure that doesn't exist({adventureId})");
            return NotFound();
        }

        if (await _db.QuestLines.AnyAsync(ql => ql.Adventure.Id == adventureId && ql.Name == Name))
        {
            ModelState.AddModelError(nameof(adventure.Name), "This questline already exists!");
        }

        _logger.LogDebug("Is modelstate valid: " + ModelState.IsValid);

        if (!ModelState.IsValid)
        {
            _logger.LogDebug($"Quest line is invalid:{Environment.NewLine}" + string.Join(Environment.NewLine,
                ModelState.SelectMany(m =>
                    m.Value?.Errors.Select(e => $"{m.Key}: {e.ErrorMessage}") ?? Array.Empty<string>())));
            this.HtmxRetargetTo("#questLineForm", "outerHTML");
            return Page();
        }

        var orders = await _db.QuestLines
            .Where(ql => ql.Adventure.Id == adventureId)
            .Select(ql => ql.Order)
            .ToListAsync();
        var nextOrder = orders.Any() ? orders.Max() + 1 : 0;
        var questLine = new Models.QuestLine
        {
            Name = Name!,
            Order = nextOrder,
            Adventure = adventure
        };

        await _db.QuestLines.AddAsync(questLine);
        await _db.SaveChangesAsync();
        _logger.LogInformation($"Created quest line {questLine}");

        return RedirectToPage("/Adventure/Index", new { id = adventureId, personId });
    }
}