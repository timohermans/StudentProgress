using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentProgress.Core.Entities;
using StudentProgress.Core.UseCases;
using System.Linq;
using System.Threading.Tasks;

namespace StudentProgress.Web.Pages.StudentGroups
{
    public class EditModel : PageModel
    {
        private readonly ProgressContext _context;
        private readonly StudentGroupUpdate _useCase;

        public EditModel(ProgressContext context)
        {
            _context = context;
            _useCase = new StudentGroupUpdate(context);
        }

        [BindProperty]
        public StudentGroupUpdate.Request StudentGroup { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var group = await _context.StudentGroup.FirstOrDefaultAsync(m => m.Id == id);


            if (group == null)
            {
                return NotFound();
            }

            StudentGroup = new StudentGroupUpdate.Request
            {
                Id = group.Id,
                Mnemonic = group.Mnemonic,
                Name = group.Name
            };
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _useCase.HandleAsync(StudentGroup);

            return RedirectToPage("./Index");
        }

        private bool StudentGroupExists(int id)
        {
            return _context.StudentGroup.Any(e => e.Id == id);
        }
    }
}
