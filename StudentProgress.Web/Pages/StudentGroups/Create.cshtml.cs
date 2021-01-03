using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentProgress.Core.Entities;
using StudentProgress.Core.UseCases;
using System.Threading.Tasks;

namespace StudentProgress.Web.Pages.StudentGroups
{
    public class CreateModel : PageModel
    {
        private readonly GroupCreate _useCase;

        public CreateModel(ProgressContext context)
        {
            _useCase = new GroupCreate(context);
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty] public GroupCreate.Request StudentGroup { get; set; } = new GroupCreate.Request();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _useCase.HandleAsync(StudentGroup);

            return RedirectToPage("./Index");
        }
    }
}