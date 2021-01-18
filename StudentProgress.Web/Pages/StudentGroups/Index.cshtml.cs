using System;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentProgress.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace StudentProgress.Web.Pages.StudentGroups
{
    public class IndexModel : PageModel
    {
        private readonly ProgressContext _context;

        public IList<StudentGroup> StudentGroup { get; set; }

        public IEnumerable<SelectListItem> Periods { get; set; }

        public IndexModel(ProgressContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(DateTime? date)
        {
            if (!date.HasValue)
            {
                return RedirectToPage("./Index", new {date = DateTime.Now.ToString("yyyy-M-d")});
            }

            var periodSelected = Period.CreateCurrentlyActivePeriodBy(date.Value).Value;
            Periods = await GetAllAvailablePeriods(periodSelected);
            StudentGroup = await _context.Groups.Where(g => g.Period == periodSelected).ToListAsync();
            return Page();
        }

        private async Task<List<SelectListItem>> GetAllAvailablePeriods(Period periodSelected)
        {
            var dbPeriods = await _context.Groups
                .Select(g => g.Period)
                .Distinct()
                .ToListAsync();

            if (!dbPeriods.Contains(periodSelected))
            {
                dbPeriods.Add(periodSelected);
            }

            return dbPeriods
                .OrderBy(p => p.StartDate)
                .Select(p => new SelectListItem(p.ToString(), p.StartDateFormattedValue, p == periodSelected))
                .ToList();
        }
    }
}