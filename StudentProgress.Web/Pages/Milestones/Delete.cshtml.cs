﻿using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentProgress.Core.Entities;
using StudentProgress.Core.UseCases;

namespace StudentProgress.Web.Pages.Milestones
{
    public class Delete : PageModel
    {
        private readonly ProgressContext _context;

        public Milestone Milestone { get; set; } = null!;
        [BindProperty] public MilestoneDelete.Command Command { get; set; } = null!;

        public Delete(ProgressContext context) => _context = context;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var milestone = await _context.Milestones
                .Include(m => m.StudentGroup)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (milestone == null)
            {
                return NotFound();
            }

            Milestone = milestone;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int groupId, CancellationToken token)
        {
            var result = await new MilestoneDelete(_context).Handle(Command, token);

            if (result.IsFailure)
            {
                ModelState.AddModelError("Summary", result.Error);
                return Page();
            }

            return RedirectToPage("/StudentGroups/Details/Index", new {Id = groupId});
        }
    }
}