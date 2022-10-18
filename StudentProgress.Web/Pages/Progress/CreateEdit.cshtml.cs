using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentProgress.Core.Entities;
using StudentProgress.Core.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StudentProgress.Core.Extensions;

namespace StudentProgress.Web.Pages.Progress
{
    public class CreateEditModel : PageModel
    {
        private readonly ProgressContext _context;
        private readonly ProgressGetForCreateOrUpdate _getUseCase;
        private readonly ProgressCreateOrUpdate _useCase;
        public ProgressGetForCreateOrUpdate.Response GetResponse { get; set; } = null!;
        public Student Student => GetResponse.Student;
        public StudentGroup Group => GetResponse.Group;
        private List<Milestone> Milestones => GetResponse.Milestones;

        public Dictionary<int, List<MilestoneProgress>> MilestoneProgressesPerMilestone => GetResponse
            .MilestoneProgresses
            .GroupBy(mp => mp.Milestone.Id)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.ToList());
        public Dictionary<string, List<Milestone>> MilestonesPerLearningOutcome => Milestones
            .GroupBy(milestone => milestone.LearningOutcome.Value)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.ToList());

        public Dictionary<string, string> LearningOutcomeNavIds => Milestones
            .Select(m => m.LearningOutcome)
            .Distinct()
            .ToDictionary(k => k.Value, l => l.Value.StripFromAllButLetters());

        [BindProperty] public ProgressCreateOrUpdate.Command Progress { get; set; } = null!;

        public CreateEditModel(ProgressContext context)
        {
            _context = context;
            _useCase = new ProgressCreateOrUpdate(context);
            _getUseCase = new ProgressGetForCreateOrUpdate(context);
        }

        public async Task<IActionResult> OnGetAsync(ProgressGetForCreateOrUpdate.Query query, CancellationToken token)
        {
            var getResult = await _getUseCase.Handle(query, token);

            if (getResult.IsFailure)
            {
                return RedirectToPage("/StudentGroups/Index");
            }

            GetResponse = getResult.Value;
            Progress = getResult.Value.Command;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string? origin, CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _useCase.Handle(Progress, token);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("UseCase", ex.Message);
                return await OnGetAsync(new ProgressGetForCreateOrUpdate.Query
                    { GroupId = Progress.GroupId, StudentId = Progress.StudentId, Id = Progress.Id }, token);
            }

            if (string.IsNullOrEmpty(origin))
            {
                return RedirectToPage("./Summary", new { Progress.StudentId, Progress.GroupId });
            }

            return Redirect(origin);
        }
    }
}