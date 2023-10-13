using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using StudentProgress.Web.Lib.Data;
using StudentProgress.Web.Lib.Extensions;
using StudentProgress.Web.Models;

namespace StudentProgress.Web.Pages.StudentGroups;

public class DeleteModel : PageModel
{
    private readonly WebContext _context;

    public DeleteModel(WebContext context) => _context = context;

    public Adventure Adventure { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var adventure = await _context.Adventures.FirstOrDefaultAsync(m => m.Id == id);

        if (adventure == null) return NotFound();

        Adventure = adventure;

        return Page();
    }

    public async Task<IActionResult> OnDeleteAsync(int? id)
    {
        await OnPostAsync(id);

        if (Request.HasHtmxTrigger($"deleteInline{id}"))
        {
            return Content("");
        }

        return this.SeeOther("/StudentGroups/Index");
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var adventure = await _context.Adventures.FindAsync(id);

        if (adventure != null)
        {
            _context.Adventures.Remove(adventure);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}