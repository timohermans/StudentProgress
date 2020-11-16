using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentProgress.Web.Models;
using System.Threading.Tasks;

namespace StudentProgress.Web.Pages.StudentGroups
{
    public class DeleteModel : PageModel
    {
        private readonly StudentProgress.Web.Data.ProgressContext _context;

        public DeleteModel(StudentProgress.Web.Data.ProgressContext context)
        {
            _context = context;
        }

        [BindProperty]
        public StudentGroup StudentGroup { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            StudentGroup = await _context.StudentGroup.FirstOrDefaultAsync(m => m.Id == id);

            if (StudentGroup == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            StudentGroup = await _context.StudentGroup.FindAsync(id);

            if (StudentGroup != null)
            {
                _context.StudentGroup.Remove(StudentGroup);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
