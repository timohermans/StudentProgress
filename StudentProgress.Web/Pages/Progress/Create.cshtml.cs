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
    public class CreateModel : PageModel
    {
        private readonly ProgressContext _context;
        private readonly Create _useCase;
        public Student Student { get; set; }
        public StudentGroup Group { get; set; }
        [BindProperty]
        public Create.Request Progress { get; set; }

        public CreateModel(ProgressContext context)
        {
            _context = context;
            _useCase = new Create(context);
        }

        public async Task<IActionResult> OnGetAsync(int? groupId = 0, int? studentId = 0)
        {
            Student = await _context.Student.FirstOrDefaultAsync(s => s.Id == studentId);
            Group = await _context.StudentGroup.FirstOrDefaultAsync(g => g.Id == groupId);

            if (Student == null || Group == null)
            {
                return RedirectToPage("/StudentGroups/Index");
            }

            Progress = new Create.Request
            {
                Date = DateTime.UtcNow,
                Feeling = Feeling.Neutral
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
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("UseCase", ex.Message);
                return await OnGetAsync(Progress.GroupId, Progress.StudentId);
            }

            return RedirectToPage("./Index", new { StudentId = Progress.StudentId, GroupId = Progress.GroupId });
        }
    }
}
