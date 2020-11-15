using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentProgress.Web.Data;
using StudentProgress.Web.Models;
using StudentProgress.Web.UseCases.StudentGroups;

namespace StudentProgress.Web.Pages.StudentGroups
{
    public class CreateModel : PageModel
    {
        private readonly Create _useCase;

        public CreateModel(StudentProgress.Web.Data.ProgressContext context)
        {
            _useCase = new Create(context);
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Create.Request StudentGroup { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
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
