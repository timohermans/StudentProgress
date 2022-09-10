using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentProgress.Core.Entities;
using StudentProgress.Core.UseCases;

namespace StudentProgress.Web.Pages.Milestones
{
    public class Delete : PageModel
    {
        private readonly ProgressContext _context;

        public Milestone? Milestone { get; set; }
        [BindProperty] public MilestoneDelete.Command Command { get; set; } = null!;

        public Delete(ProgressContext context) => _context = context;

        public async Task<IActionResult> OnGetAsync(int milestoneId)
        {
            Milestone = await _context.Milestones
                .Include(m => m.StudentGroup)
                .FirstOrDefaultAsync(m => m.Id == milestoneId);

            if (Milestone == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int groupId)
        {
            var result = await new MilestoneDelete(_context).HandleAsync(Command);

            if (result.IsFailure)
            {
                ModelState.AddModelError("Summary", result.Error);
                return Page();
            }

            return RedirectToPage("/StudentGroups/Details/Index", new {Id = groupId});
        }
    }
}