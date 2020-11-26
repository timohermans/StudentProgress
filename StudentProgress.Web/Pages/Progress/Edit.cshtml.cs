using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentProgress.Web.Data;
using StudentProgress.Web.Models;
using StudentProgress.Web.UseCases.Progress;
using System;
using System.Threading.Tasks;

namespace StudentProgress.Web.Pages.Progress
{
    public class EditModel : PageModel
    {
        private readonly ProgressContext _context;
        private readonly Edit _useCase;
        public Student Student { get; set; }
        public StudentGroup Group { get; set; }
        [BindProperty]
        public Edit.Request Progress { get; set; }

        public EditModel(ProgressContext context)
        {
            _context = context;
            _useCase = new Edit(context);
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var progress = await _context.ProgressUpdate.Include(p => p.Student).Include(p => p.Group).FirstOrDefaultAsync(p => p.Id == id);

            if (progress == null)
            {
                return NotFound();
            }

            Student = progress.Student;
            Group = progress.Group;

            Progress = new Edit.Request
            {
                Id = progress.Id,
                Feeling = progress.ProgressFeeling,
                Date = progress.Date,
                Feedforward = progress.Feedforward,
                Feedback = progress.Feedback,
                Feedup = progress.Feedup
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _useCase.HandleAsync(Progress);

                var progress = await _context.ProgressUpdate.FindAsync(Progress.Id);
                return RedirectToPage("./Index", new { StudentId = progress.StudentId, GroupId = progress.GroupId });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("UseCase", ex.Message);
                return await OnGetAsync(Progress.Id);
            }
        }
    }
}
