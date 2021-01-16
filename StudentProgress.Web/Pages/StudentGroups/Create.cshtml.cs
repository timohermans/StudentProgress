using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentProgress.Core.Entities;
using StudentProgress.Core.UseCases;
using System.Threading.Tasks;
using StudentProgress.Core.Extensions;

namespace StudentProgress.Web.Pages.StudentGroups
{
    public class CreateModel : PageModel
    {
        private readonly GroupCreate _useCase;

        [BindProperty] public GroupCreate.Request StudentGroup { get; set; }

        public CreateModel(ProgressContext context)
        {
            _useCase = new GroupCreate(context);
        }

        public IActionResult OnGet()
        {
            StudentGroup = new()
            {
                StartDate = Period
                    .Create(new DateTime(DateTime.Now.Year, DateTime.Now.Month.IsInRange(2, 9) ? 9 : 2, 1)).Value
                    .StartDate
            };
            return Page();
        }

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