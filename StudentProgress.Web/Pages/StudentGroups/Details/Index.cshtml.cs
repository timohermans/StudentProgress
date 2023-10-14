using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentProgress.Core.UseCases;
using StudentProgress.Core.Entities;

namespace StudentProgress.Web.Pages.StudentGroups.Details
{
    public class IndexModel : PageModel
    {
        private readonly StudentGroupGetDetails _useCase;
        private readonly MilestonesUpdateLearningOutcome _milestonesUpdateUseCase;

        public IndexModel(ProgressContext context)
        {
            _useCase = new StudentGroupGetDetails(context);
            _milestonesUpdateUseCase = new MilestonesUpdateLearningOutcome(context);
        }

        public bool IsSortedOnLastFeedback { get; set; }
        public StudentGroupGetDetails.Response StudentGroup { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int? id, string? sort, CancellationToken token)
        {
            if (id == null)
            {
                return NotFound();
            }

            // var studentGroup = await _useCase.Handle(new StudentGroupGetDetails.Request((int)id), token);

            // if (studentGroup == null)
            // {
            //     return NotFound();
            // }

            // if (sort == "last-feedback")
            // {
            //     IsSortedOnLastFeedback = true;
            //     studentGroup.Students = studentGroup.Students
            //         .OrderByDescending(s => s.ProgressUpdates
            //             .Select(u => u.Date)
            //             .DefaultIfEmpty(DateTime.MinValue)
            //             .Max(p => p.Date))
            //         .ToList();
            // }

            // StudentGroup = studentGroup;
            
            return Page();
        }

        public async Task<IActionResult> OnPostUpdateMultipleMilestonesAsync(MilestonesUpdateLearningOutcome.Command command, CancellationToken token)
        {
            var result = await _milestonesUpdateUseCase.Handle(command, token);

            if (result.IsFailure)
            {
                ModelState.AddModelError("MilestoneSummary", result.Error);
            }
            
            return await OnGetAsync(command.GroupId, null, token);
        }
    }
}
