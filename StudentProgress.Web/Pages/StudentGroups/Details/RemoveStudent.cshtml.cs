using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentProgress.Core.Entities;
using StudentProgress.Core.UseCases;

namespace StudentProgress.Web.Pages.StudentGroups.Details
{
    public class RemoveStudent : PageModel
    {
        private readonly ProgressContext _context;

        public Student Student { get; set; }
        public StudentGroup Group { get; set; }
        
        [BindProperty]
        public StudentRemoveFromGroup.Command Command { get; set; }

        public RemoveStudent(ProgressContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(int studentId, int groupId)
        {
            var studentResult = Maybe<Student>.From(await _context.Students.FindAsync(studentId));
            var groupResult = Maybe<StudentGroup>.From(await _context.Groups.FindAsync(groupId));

            if (studentResult.HasNoValue || groupResult.HasNoValue) return RedirectToPage("/Index");

            Student = studentResult.Value;
            Group = groupResult.Value;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await new StudentRemoveFromGroup(_context).HandleAsync(Command);

            return RedirectToPage("./Index", new {Id = Command.GroupId});
        }
    }
}