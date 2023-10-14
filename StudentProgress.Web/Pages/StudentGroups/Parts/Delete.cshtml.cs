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

    public async Task<IActionResult> OnDeleteAsync(int id)
    {
        var adventure = await _context.Adventures.FindAsync(id);
        
        if (adventure != null)
        {
            _context.Adventures.Remove(adventure);
            await _context.SaveChangesAsync();
        }

        if (Request.HasHtmxTrigger($"deleteInline{id}"))
        {
            return Page();
        }

        return this.SeeOther("../Index");
    }
}