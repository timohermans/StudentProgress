using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudentProgress.Web.Lib.Data;

namespace StudentProgress.Web.Pages.QuestLine;

public class Index : PageModel
{
    private readonly ILogger<Index> _logger;
    private readonly WebContext _db;
    
    public int Id { get; set; }
    public required Models.QuestLine QuestLine { get; set; }

    public Index(ILogger<Index> logger, WebContext db)
    {
        _logger = logger;
        _db = db;
    }

    public async Task<IActionResult> OnGet(int id)
    {
        _logger.LogDebug($"Questline fetch for {id}");
        if (id == 0) return new EmptyResult();
        Id = id;

        var questLine = await _db.QuestLines
            .Include(ql => ql.Quests)
            .FirstOrDefaultAsync(q => q.Id == id);

        if (questLine == null) return NotFound();

        QuestLine = questLine;

        return Page();
    }
}