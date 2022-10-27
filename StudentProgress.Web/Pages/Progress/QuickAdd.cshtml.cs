using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentProgress.Core.Entities;
using StudentProgress.Core.UseCases;

namespace StudentProgress.Web.Pages.Progress;

public class QuickAdd : PageModel
{
    private readonly ProgressContext _db;

    [BindProperty] public QuickAddCommand? Command { get; set; }
    public IList<SelectListItem> Groups { get; set; } = null!;

    public StudentGroup? Group { get; set; }
    public List<SelectListItem> Milestones { get; set; } = new();

    public QuickAdd(ProgressContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> OnGet(int? groupId)
    {
        var groups = await _db
            .Groups
            .OrderByDescending(g => g.Period)
            .ToListAsync();

        Groups = groups
            .Select(g => new SelectListItem(g.Name, g.Id.ToString(), g.Id == groupId))
            .ToList();
        Groups.Insert(0, new SelectListItem("-- Select a group --", null, groupId == null));

        if (groupId.HasValue)
        {
            Group = await _db
                .Groups
                .Include(g => g.Milestones)
                .Include(g => g.Students)
                .ThenInclude(s => s.ProgressUpdates)
                .FirstAsync(g => g.Id == groupId);
            Milestones = Group
                .Milestones
                .OrderBy(m => m.LearningOutcome)
                .ThenBy(m => m.Artefact)
                .Select(m => new SelectListItem(m.ToString(), m.Id.ToString()))
                .ToList();
            Milestones.Insert(0, new SelectListItem("-- Select a milestone --", null));

            Command ??= new QuickAddCommand
            {
                GroupId = groupId.Value,
                Students = Group.Students.Select(s => new QuickAddCommand.StudentCommand(s.Id, null, null)).ToList()
            };
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? groupId)
    {
        if (!ModelState.IsValid || !groupId.HasValue)
        {
            return await OnGet(groupId);
        }

        var group = await _db
            .Groups
            .Include(g => g.Students)
            .Include(g => g.Milestones)
            .FirstOrDefaultAsync(g => g.Id == groupId);

        if (group == null || Command == null)
        {
            return NotFound();
        }

        var progressUpdatesToCreate = Command
            .Students
            .Where(s => s.Rating.HasValue || !string.IsNullOrWhiteSpace(s.Comment))
            .Select(s =>
            {
                var student = group.Students.First(dbStudent => dbStudent.Id == s.StudentId);
                var milestone = group.Milestones.First(m => m.Id == Command.MilestoneId);
                var rating = s.Rating ?? Rating.Orienting; 
                var progressUpdate = new ProgressUpdate(
                    student,
                    group,
                    null,
                    (int)rating switch
                    {
                        < 3 => Feeling.Bad,
                        3 => Feeling.Neutral,
                        > 3 => Feeling.Good
                    },
                    DateTime.Now, 
                    true);

                progressUpdate.AddMilestones(new[] { new MilestoneProgress(rating, milestone, s.Comment) });
                return progressUpdate;
            })
            .ToList();

        await _db.ProgressUpdates.AddRangeAsync(progressUpdatesToCreate);
        await _db.SaveChangesAsync();

        return RedirectToPage("/StudentGroups/Details/Index", new { Id = Command.GroupId, Sort = "last-feedback" });
        // TODO: unit test this
        // TODO: refactor to core
    }

    public class QuickAddCommand : IUseCaseRequest<Result>
    {
        [Required] public int GroupId { get; init; }
        [Required] public int MilestoneId { get; init; }
        public List<StudentCommand> Students { get; init; } = new();

        public record StudentCommand(int StudentId, Rating? Rating, string? Comment);
    }
}