using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        public StudentGroup Group { get; set; } = null!;
        public List<StudentGroup> OtherGroups { get; set; } = new();
        [BindProperty] public MilestoneCreate.Command Milestone { get; set; } = null!;
        [BindProperty] public MilestonesCopyFromGroup.Command CopyCommand { get; set; } = null!;

        public AddMilestone(ProgressContext context) => _context = context;

        public async Task<IActionResult> OnGet(int id, string? returnUrl = null)
        {
            var group = Maybe<StudentGroup?>.From(await _context
                .Groups
                .Include(g => g.Milestones)
                .FirstOrDefaultAsync(g => g.Id == id));

            if (group.HasNoValue)
            {
                return RedirectToPage("/StudentGroups/Index");
            }

            Group = group.Value!;
            OtherGroups = _context.Groups.Where(g => g.Id != group.Value!.Id).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl, CancellationToken token)
        {
            return await CreateMilestone(milestoneId =>
            {
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect($"{returnUrl}&milestoneId={milestoneId}");
                }
                
                return RedirectToPage("./Index", new { Id = Milestone.GroupId });
            }, token);
        }

        public async Task<IActionResult> OnPostAndAddAnother(CancellationToken token)
        {
            return await CreateMilestone(_ => RedirectToPage("./AddMilestone", new {Id = Milestone.GroupId}), token);
        }

        private async Task<IActionResult> CreateMilestone(Func<int, IActionResult> onSuccessFunc, CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                return await OnGet(Milestone.GroupId);
            }

            var result = await new MilestoneCreate(_context).Handle(Milestone, token);

            if (result.IsFailure)
            {
                return await HandleError(result);
            }

            return onSuccessFunc.Invoke(result.Value);
        }

        private async Task<IActionResult> HandleError(Result result)
        {
            ModelState.AddModelError("Summary", result.Error);
            return await OnGet(Milestone.GroupId);
        }

        public async Task<IActionResult> OnPostCopyFromGroup(CancellationToken token)
        {
            var result = await new MilestonesCopyFromGroup(_context).Handle(CopyCommand, token);

            if (result.IsFailure)
            {
                return await HandleError(result);
            }

            return RedirectToPage("./Index", new {Id = CopyCommand.ToGroupId});
        }
    }
}