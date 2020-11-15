using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentProgress.Web.Data;
using StudentProgress.Web.UseCases.Progress;

namespace StudentProgress.Web.Pages.Progress
{
    public class IndexModel : PageModel
    {
        private readonly GetProgressForStudentInGroup _useCase;

        public IndexModel(ProgressContext context)
        {
            _useCase = new GetProgressForStudentInGroup(context);
        }

        public GetProgressForStudentInGroup.Response Student { get; set; }

        public async Task<IActionResult> OnGetAsync(GetProgressForStudentInGroup.Request request)
        {
            Student = await _useCase.HandleAsync(request);
            return Page();
        }
    }
}
