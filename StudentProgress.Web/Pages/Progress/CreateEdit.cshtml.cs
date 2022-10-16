using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentProgress.Core.Entities;
using StudentProgress.Core.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace StudentProgress.Web.Pages.Progress
{
    public class CreateEditModel : PageModel
    {
        private readonly ProgressContext _context;
        private readonly ProgressGetForCreateOrUpdate _getUseCase;
        private readonly ProgressCreateOrUpdate _useCase;
        private readonly ProgressGetSummaryForStudentInGroup _summaryUseCase;
        public ProgressGetForCreateOrUpdate.Response GetResponse { get; set; } = null!;
        public ProgressGetSummaryForStudentInGroup.Response Summary { get; set; } = null!;
        public Student Student => GetResponse.Student;
        public StudentGroup Group => GetResponse.Group;
        public List<Milestone> Milestones => GetResponse.Milestones;

        public Dictionary<string, string> MilestoneNavIds => Milestones
            .Select(m => m.LearningOutcome)
            .Distinct()
            .ToDictionary(k => k.Value, l => Regex.Replace(l.Value, @"[^a-zA-Z]", string.Empty));

        [BindProperty] public ProgressCreateOrUpdate.Command Progress { get; set; } = null!;

        public CreateEditModel(ProgressContext context)
        {
            _context = context;
            _useCase = new ProgressCreateOrUpdate(context);
            _getUseCase = new ProgressGetForCreateOrUpdate(context);
            _summaryUseCase = new ProgressGetSummaryForStudentInGroup(context);
        }

        public async Task<IActionResult> OnGetAsync(ProgressGetForCreateOrUpdate.Query query, CancellationToken token)
        {
            var getResult = await _getUseCase.Handle(query, token);
            // TODO: in the end, refactor this into the progress get for create or update
            var summaryResult = await _summaryUseCase.Handle(new ProgressGetSummaryForStudentInGroup.Query
            {
                GroupId = query.GroupId,
                StudentId = query.StudentId
            }, token);

            if (getResult.IsFailure || summaryResult.IsFailure)
            {
                return RedirectToPage("/StudentGroups/Index");
            }

            GetResponse = getResult.Value;
            Progress = getResult.Value.Command;
            Summary = summaryResult.Value;

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