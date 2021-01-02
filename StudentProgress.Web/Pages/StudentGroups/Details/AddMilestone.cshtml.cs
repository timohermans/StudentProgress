using System;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentProgress.Core.Entities;
using StudentProgress.Core.UseCases;

namespace StudentProgress.Web.Pages.StudentGroups.Details
{
    public class AddMilestone : PageModel
    {
        private readonly ProgressContext _context;
        public StudentGroup Group { get; set; }
        [BindProperty] public MilestoneCreate.Request Milestone { get; set; }

        public AddMilestone(ProgressContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGet(int id)
        {
            var group = Maybe<StudentGroup>.From(await _context
                .Groups
                .Include(g => g.Milestones)
                .FirstOrDefaultAsync(g => g.Id == id));

            if (group.HasNoValue)
            {
                return RedirectToPage("/StudentGroups/Index");
            }

            Group = group.Value;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            return await CreateMilestone(() => RedirectToPage("./Index", new {Id = Milestone.GroupId}));
        }

        public async Task<IActionResult> OnPostAndAddAnother()
        {
            return await CreateMilestone(() => RedirectToPage("./AddMilestone", new {Id = Milestone.GroupId}));
        }

        private async Task<IActionResult> CreateMilestone(Func<IActionResult> onSuccessFunc)
        {
            if (!ModelState.IsValid)
            {
                return await OnGet(Milestone.GroupId);
            }

            var result = await new MilestoneCreate(_context).HandleAsync(Milestone);

            if (result.IsFailure)
            {
                ModelState.AddModelError("Summary", result.Error);
                return await OnGet(Milestone.GroupId);
            }

            return onSuccessFunc.Invoke();
        }
    }
}