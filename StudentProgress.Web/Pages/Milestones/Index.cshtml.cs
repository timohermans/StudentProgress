using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentProgress.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StudentProgress.Web.Pages.Milestones;

public class IndexViewModel
{
    public required int GroupId { get; set; }
    public required string GroupName { get; set; }
    public required IEnumerable<StudentViewModel> Students { get; set; }
    public required IEnumerable<MilestoneHeaderViewModel> Milestones { get; set; }
    public required int? MilestoneIdSelected { get; set; }
    public IEnumerable<SelectListItem> MilestonesToSelect
    {
        get
        {
            var items = Milestones.Select(m => new SelectListItem(m.Name, m.Id.ToString(), m.Id == MilestoneIdSelected)).ToList();
            items.Insert(0, new SelectListItem("-- all milestones --", null));
            return items;
        }
    }
}

public class StudentViewModel
{
    public string? AvatarPath { get; set; }
    public required string Name { get; set; }
    public required IEnumerable<MilestoneViewModel> Milestones { get; set; }
}

public class MilestoneViewModel
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public int AmountOfUpdates => AllRatings.Count();
    public required IEnumerable<(DateTime Date, Rating Rating)> AllRatings { get; set; }
    public required Rating? LatestRating { get; set; }
}

public class MilestoneHeaderViewModel
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public int AmountOfStudentsRated { get; set; }
    public required int AmountOfStudentsTotal { get; set; }
}

public class IndexModel : PageModel
{
    private readonly ProgressContext _context;

    public required IndexViewModel ViewModel { get; set; }

    public IndexModel(ProgressContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> OnGet(int groupId, int? milestoneId, CancellationToken token)
    {
        var group = await _context
            .Groups
            .Include(g => g.Milestones.Where(m => milestoneId == null || m.Id == milestoneId))
            .Include(g => g.Students)
                .ThenInclude(s => s.ProgressUpdates.Where(pu => pu.GroupId == groupId))
                .ThenInclude(p => p.MilestonesProgress)
            .AsSplitQuery()
            .FirstOrDefaultAsync(g => g.Id == groupId, token);

        if (group == null) return NotFound();

        var students = group.Students;

        ViewModel = new IndexViewModel
        {
            GroupId = group.Id,
            GroupName = group.Name,
            MilestoneIdSelected = milestoneId,
            Students = students.Select(s =>
            {

                return new StudentViewModel
                {
                    AvatarPath = s.AvatarPath,
                    Name = s.Name,
                    Milestones = group.Milestones.Select(m =>
                    {
                        var updates = s
                        .ProgressUpdates
                        .SelectMany(pu => pu.MilestonesProgress)
                        .Where(mp => mp.MilestoneId == m.Id)
                        .ToList();
                        var latestUpdate = updates.MaxBy(mp => mp.CreatedDate);

                        return new MilestoneViewModel
                        {
                            Id = m.Id,
                            Name = m.ToString(),
                            AllRatings = updates.Select(mp => (mp.CreatedDate, mp.Rating)).ToList(),
                            LatestRating = latestUpdate?.Rating
                        };
                    })

                };
            }).ToList(),
            Milestones = group.Milestones.Select(m =>
            {

                return new MilestoneHeaderViewModel
                {
                    Id = m.Id,
                    Name = m.ToString(),
                    AmountOfStudentsTotal = students.Count,
                    AmountOfStudentsRated = students
                        .Where(s => s.ProgressUpdates
                                        .Any(pu => pu.MilestonesProgress
                                        .Any(mp => mp.MilestoneId == m.Id)))
                        .Count()
                };
            }).ToList(),
        };


        return Page();
    }
}
