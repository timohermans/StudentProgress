using System.Linq;
using System.Threading;
using LanguageExt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudentProgress.Core.Data;
using StudentProgress.Web.Lib.Extensions;
using static LanguageExt.Prelude;
using Adventure = StudentProgress.Core.Models.Adventure;

namespace StudentProgress.Web.Pages.Adventures;

public class EditModel : PageModel
{
    private readonly WebContext _context;
    private readonly ILogger<EditModel> _logger;

    [BindProperty] public Adventure Adventure { get; set; } = null!;

    public EditModel(WebContext context, ILogger<EditModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var adventure = await _context.Adventures.FindAsync(id);

        if (adventure == null)
        {
            return NotFound();
        }

        Adventure = adventure;

        return Page();
    }

    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see https://aka.ms/RazorPagesCRUD.
    public async Task<IActionResult> OnPutAsync(CancellationToken token)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var adventure = Right<(string modelKey, string modelValue), Adventure>(Adventure);
        var exists = adventure.Bind(MaybeExists);
        var duplicate = await exists.BindAsync(MaybeDuplicate);
        var update = await duplicate.BindAsync(Update);

        return update.Match(
            Left: result =>
            {
                this.HtmxRetargetTo($"#adventure-{Adventure.Id}-options", "innerHTML");
                ModelState.AddModelError(result.modelKey, result.modelValue);
                return base.Partial("_Form", (object)Adventure);
            },
            Right: a => Partial("_Row", a));
    }

    private Either<(string modelKey, string modelValue), Adventure> MaybeExists(Adventure a)
        => _context.Adventures.Any(x => x.Id == a.Id)
            ? a
            : (nameof(Adventure), "Something went wrong");

    private async Task<Either<(string modelKey, string modelValue), Adventure>> MaybeDuplicate(
        Adventure a)
        => await _context.Adventures.AnyAsync(x => x.Name == a.Name)
            ? (nameof(Adventure.Name), "Already exists!")
            : a;

    private async Task<Either<(string modelKey, string modelValue), Adventure>> Update(Adventure a)
    {
        try
        {
            _context.Attach(a).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return a;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Something went wrong saving the adventure");
            return (nameof(Adventure), "Something went wrong saving the adventure");
        }
    }
}