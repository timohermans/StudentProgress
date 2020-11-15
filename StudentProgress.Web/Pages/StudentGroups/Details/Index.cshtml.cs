using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentProgress.Web.UseCases.StudentGroups;
using System.Threading.Tasks;

namespace StudentProgress.Web.Pages.StudentGroups.Details
{
    public class IndexModel : PageModel
    {
        private readonly GetStudentGroupDetails _useCase;

        public IndexModel(StudentProgress.Web.Data.ProgressContext context)
        {
            _useCase = new GetStudentGroupDetails(context);
        }

        public GetStudentGroupDetails.Response StudentGroup { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            StudentGroup = await _useCase.HandleAsync(new GetStudentGroupDetails.Request((int)id));

            if (StudentGroup == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
