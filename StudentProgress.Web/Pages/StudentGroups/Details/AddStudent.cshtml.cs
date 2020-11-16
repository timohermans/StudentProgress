using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentProgress.Web.Data;
using StudentProgress.Web.Models;
using StudentProgress.Web.UseCases.Students;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace StudentProgress.Web.Pages.StudentGroups.Details
{
    public class AddStudentModel : PageModel
    {
        private readonly ProgressContext _context;
        private readonly AddToGroup _useCase;

        public StudentGroup Group { get; set; }

        public AddStudentModel(StudentProgress.Web.Data.ProgressContext context)
        {
            _context = context;
            _useCase = new AddToGroup(_context);
        }

        public IActionResult OnGet(int? groupId)
        {
            Group = _context.StudentGroup.FirstOrDefault(g => g.Id == (groupId ?? 0));
            if (Group == null)
            {
                RedirectToPage("/StudentGroups/Index");
            }

            return Page();
        }

        public int? GroupId { get; set; }

        [BindProperty]
        public AddToGroup.Request Student { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _useCase.HandleAsync(Student);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("Summary", ex.Message);
                return OnGet(Student.GroupId);
            }

            return RedirectToPage("./Index", new { Id = Student.GroupId });
        }
    }
}
