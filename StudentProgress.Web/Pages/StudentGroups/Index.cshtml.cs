using System;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentProgress.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentProgress.Core.CanvasApi;

namespace StudentProgress.Web.Pages.StudentGroups
{
    public class IndexModel : PageModel
    {
        private readonly ProgressContext _context;
        private readonly ICanvasApiConfig _apiConfig;

        public bool CanImportGroups { get; private set; }
        public List<StudentGroup> StudentGroups { get; set; } = new();

        public List<SelectListItem> Periods { get; set; } = new();
        public Period CurrentPeriod { get; set; } = null!;

        public IndexModel(ProgressContext context, ICanvasApiConfig apiConfig)
        {
            _context = context;
            _apiConfig = apiConfig;
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
            return Page();
        }
    }
}