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
    public class CreateModel : PageModel
    {
        private readonly ProgressContext _context;
        private readonly ProgressCreate _useCase;
        public Student Student { get; set; }
        public StudentGroup Group { get; set; }
        public Dictionary<string, List<Milestone>> MilestonesPerLearningOutcome { get; set; }
        [BindProperty] public ProgressCreate.Request Progress { get; set; }

        public CreateModel(ProgressContext context)
        {
            _context = context;
            _useCase = new ProgressCreate(context);
        }

        public async Task<IActionResult> OnGetAsync(int? groupId = 0, int? studentId = 0)
        {
            Student = await _context.Students.FirstOrDefaultAsync(s => s.Id == studentId);
            Group = await _context.Groups.FirstOrDefaultAsync(g => g.Id == groupId);

            if (Student == null || Group == null)
            {
                return RedirectToPage("/StudentGroups/Index");
            }

            var milestones = _context.Milestones
                .Where(m => m.StudentGroup.Id == groupId)
                .OrderBy(m => m.LearningOutcome)
                .ToList();
            MilestonesPerLearningOutcome = milestones
                .GroupBy(g => Regex.Replace(g.LearningOutcome.Value, @"[^a-zA-Z]", String.Empty))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.ToList());
            Progress = new ProgressCreate.Request
            {
                Date = DateTime.UtcNow,
                Feeling = Feeling.Neutral,
                Milestones = milestones.Select(m => new ProgressCreate.MilestoneProgress
                {
                    Rating = null,
                    MilestoneId = m.Id
                }).ToList()
            };

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
                return await OnGetAsync(Progress.GroupId, Progress.StudentId);
            }

            return RedirectToPage("./Index", new {StudentId = Progress.StudentId, GroupId = Progress.GroupId});
        }
    }
}