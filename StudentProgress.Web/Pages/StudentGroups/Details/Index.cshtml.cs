﻿using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentProgress.Core.UseCases;
using System.Threading.Tasks;
using StudentProgress.Core.Entities;

namespace StudentProgress.Web.Pages.StudentGroups.Details
{
    public class IndexModel : PageModel
    {
        private readonly StudentGroupGetDetails _useCase;
        private readonly MilestonesUpdateLearningOutcome _milestonesUpdateUseCase;

        public IndexModel(IDbConnection connection, ProgressContext context)
        {
            _useCase = new StudentGroupGetDetails(connection, context);
            _milestonesUpdateUseCase = new MilestonesUpdateLearningOutcome(context);
        }

        public StudentGroupGetDetails.Response StudentGroup { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            StudentGroup = await _useCase.HandleAsync(new StudentGroupGetDetails.Request((int)id));

            if (StudentGroup == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostUpdateMultipleMilestonesAsync(MilestonesUpdateLearningOutcome.Command command)
        {
            var result = await _milestonesUpdateUseCase.HandleAsync(command);

            if (result.IsFailure)
            {
                ModelState.AddModelError("MilestoneSummary", result.Error);
            }
            
            return await OnGetAsync(command.GroupId);
        }
    }
}
