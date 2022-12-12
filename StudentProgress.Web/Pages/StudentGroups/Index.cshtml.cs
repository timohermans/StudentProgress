using System;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentProgress.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentProgress.Core;
using StudentProgress.Core.CanvasApi;
using StudentProgress.Core.Extensions;

namespace StudentProgress.Web.Pages.StudentGroups
{
    public class IndexModel : PageModel
    {
        private readonly ProgressContext _context;
        private readonly ICanvasApiConfig _apiConfig;

        [BindProperty] public required Request Update { get; set; }

        public bool CanImportGroups { get; private set; }
        public IDateProvider DateProvider;
        public List<StudentGroup> StudentGroups { get; set; } = new();
        public List<SelectListItem> Periods { get; set; } = new();
        public List<ProgressToReview> ProgressesToReview { get; set; } = new();
        public Period CurrentPeriod { get; set; } = null!;

        public IndexModel(ProgressContext context, ICanvasApiConfig apiConfig, IDateProvider dateProvider)
        {
            _context = context;
            _apiConfig = apiConfig;
            DateProvider = dateProvider;
        }

        public async Task<IActionResult> OnGetAsync(DateTime? date)
        {
            if (!date.HasValue)
            {
                return RedirectToPage("./Index", new {date = DateTime.Now.ToString("yyyy-M-d")});
            }

            CanImportGroups = await _apiConfig.CanUseCanvasApiAsync();
            CurrentPeriod = Period.CreateCurrentlyActivePeriodBy(DateTime.Today).Value;
            var groups = await _context.Groups
                                         .OrderByDescending(g => g.Period)
                                         .ToListAsync();

            StudentGroups = groups.Where(g => g.Period.IsVeryOldDate).ToList();
            StudentGroups.AddRange(groups.Where(g => !g.Period.IsVeryOldDate).ToList());

            ProgressesToReview = await _context
                .ProgressUpdates
                .Include(p => p.Student)
                .Include(p => p.Group)
                .Where(pu => !pu.IsReviewed)
                .OrderBy(pu => pu.Date)
                .Select(pu => new ProgressToReview(
                    pu.Id,
                    pu.StudentId,
                    pu.GroupId,
                    pu.Date.TimePassedShort(DateProvider),
                    pu.Student.Name,
                    pu.Group.Name.GetFirstPart(' ')
                    ))
                .ToListAsync();
            
            return Page();
        }

        public async Task<IActionResult> OnPostMarkReviewedAsync(DateTime? date)
        {
            var update = await _context.ProgressUpdates.FindAsync(Update.Id);
            if (update == null) throw new InvalidOperationException();
            update.Update(update.ProgressFeeling, update.Date, update.Feedback, true);
            await _context.SaveChangesAsync();
            return RedirectToPage("/StudentGroups/Index");
        }
    }

    public record ProgressToReview(int Id, int StudentId, int GroupId, string DaysAgo, string Student, string Course);

    public class Request
    {
        public int Id { get; set; }
    }
}