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
using System.Threading.Tasks;

namespace StudentProgress.Web.Pages.Progress
{
    public class CreateEditModel : PageModel
    {
        private readonly ProgressContext _context;
        private readonly ProgressGetForCreateOrUpdate _getUseCase;
        private readonly ProgressCreateOrUpdate _useCase;
        public ProgressGetForCreateOrUpdate.Response GetResponse { get; set; }
        public Student Student => GetResponse.Student;
        public StudentGroup Group => GetResponse.Group;
        public List<Milestone> Milestones => GetResponse.Milestones;
        public Dictionary<string, string> MilestoneNavIds => Milestones
                .Select(m => m.LearningOutcome)
                .Distinct()
                .ToDictionary(k => k.Value, l => Regex.Replace(l.Value, @"[^a-zA-Z]", string.Empty));
        [BindProperty] public ProgressCreateOrUpdate.Command Progress { get; set; }

        public CreateEditModel(ProgressContext context)
        {
            _context = context;
            _useCase = new ProgressCreateOrUpdate(context);
            _getUseCase = new ProgressGetForCreateOrUpdate(context);
        }

        public async Task<IActionResult> OnGetAsync(ProgressGetForCreateOrUpdate.Query query)
        {
            var getResult = await _getUseCase.HandleAsync(query);

            if (getResult.IsFailure)
            {
                return RedirectToPage("/StudentGroups/Index");
            }

            GetResponse = getResult.Value;
            Progress = getResult.Value.Command;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _useCase.HandleAsync(Progress);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("UseCase", ex.Message);
                return await OnGetAsync(new ProgressGetForCreateOrUpdate.Query { GroupId = Progress.GroupId, StudentId = Progress.StudentId, Id = Progress.Id });
            }

            return RedirectToPage("./Summary", new { Progress.StudentId, Progress.GroupId });
        }
    }
}