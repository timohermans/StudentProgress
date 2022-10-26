using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentProgress.Core.Entities;
using StudentProgress.Core.UseCases;

namespace StudentProgress.Web.Pages.Progress;

public class QuickAdd : PageModel
{
    private readonly ProgressContext _db;

    [BindProperty] public QuickAddCommand Command { get; set; } = null!;
    public IEnumerable<StudentGroup> Groups { get; set; } = null!;

    public StudentGroup? Group { get; set; }

    public QuickAdd(ProgressContext db)
    {
        _db = db;
    }

    public async Task OnGet(int? groupId)
    {
        Groups = await _db
            .Groups
            .OrderByDescending(g => g.Period)
            .ToListAsync();

        if (groupId.HasValue)
        {
            Group = await _db
                .Groups
                .Include(g => g.Milestones)
                .Include(g => g.Students)
                .ThenInclude(s => s.ProgressUpdates)
                .FirstAsync(g => g.Id == groupId);
            Command = new QuickAddCommand { GroupId = groupId.Value };
        }
    }

    public class QuickAddCommand : IUseCaseRequest<Result>
    {
        [Required] public int GroupId { get; init; }
        [Required] public int MilestoneId { get; init; }
    }
}