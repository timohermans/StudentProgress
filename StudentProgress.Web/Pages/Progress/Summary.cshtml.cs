using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentProgress.Core.Entities;
using StudentProgress.Core.UseCases;

namespace StudentProgress.Web.Pages.Progress
{
  public class SummaryModel : PageModel
  {
    private readonly ProgressGetSummaryForStudentInGroup _useCase;

    public ProgressGetSummaryForStudentInGroup.Response Summary { get; set; } = null!;

    public SummaryModel(ProgressContext context) => _useCase = new ProgressGetSummaryForStudentInGroup(context);

    public async Task<IActionResult> OnGetAsync(ProgressGetSummaryForStudentInGroup.Query query, CancellationToken token)
    {
      var summaryResult = await _useCase.Handle(query, token);

      if (summaryResult.IsFailure)
      {
        return RedirectToPage("./StudentGroup/Index");
      }

      Summary = summaryResult.Value;
      return Page();
    }

  }
}