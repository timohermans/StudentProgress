using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentProgress.Core.Entities;
using StudentProgress.Core.UseCases;
using StudentProgress.Web.Lib.Data;
using StudentProgress.Web.Models;

namespace StudentProgress.Web.Pages.StudentGroups.Parts;

public class EditModel : PageModel
{
    private readonly WebContext _context;

    [BindProperty] public Adventure Adventure { get; set; } = null!;

    public EditModel(WebContext context)
    {
        _context = context;
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

        _context.Attach(Adventure).State = EntityState.Modified;
        await _context.SaveChangesAsync(token);

        return Partial("_AdventureRow", Adventure);
    }
}