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
        public List<Milestone> Milestones { get; set; }
        [BindProperty] public ProgressCreate.Command Progress { get; set; }

        public CreateModel(ProgressContext context)
        {
            _context = context;
            _useCase = new ProgressCreate(context);
        }

        public async Task<IActionResult> OnGetAsync(int? groupId = 0, int? studentId = 0)
        {
            Student = await _context.Students.FirstOrDefaultAsync(s => s.Id == studentId);
            Group = await _context.Groups.FirstOrDefaultAsync(g => g.Id == groupId);
            Milestones = _context.Milestones
                .Where(m => m.StudentGroup.Id == groupId)
                .OrderBy(m => m.LearningOutcome)
                .ToList();

            if (Student == null || Group == null)
            {
                return RedirectToPage("/StudentGroups/Index");
            }

            Progress = new ProgressCreate.Command
            {
                Date = DateTime.UtcNow,
                Feeling = Feeling.Neutral,
                Milestones = Milestones.Select(m => new ProgressCreate.MilestoneProgressCommand
                {
                    Rating = null,
                    Id = m.Id
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