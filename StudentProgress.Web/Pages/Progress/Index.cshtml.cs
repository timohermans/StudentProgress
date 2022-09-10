using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentProgress.Core.Entities;
using StudentProgress.Core.UseCases;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentProgress.Web.Pages.Progress
{ 
    public class IndexModel : PageModel
    {
        private readonly ProgressGetForStudentInGroup _useCase;

        public ProgressGetForStudentInGroup.Response Student { get; set; } = null!;

        public IndexModel(ProgressContext context) => _useCase = new ProgressGetForStudentInGroup(context);

        public async Task<IActionResult> OnGetAsync(ProgressGetForStudentInGroup.Request request)
        {
            Student = await _useCase.HandleAsync(request);
            return Page();
        }
    }
}
