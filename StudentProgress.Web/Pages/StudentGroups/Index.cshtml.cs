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

        public IList<StudentGroup> StudentGroups { get; set; }

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

            StudentGroups = await _context.Groups
                .OrderByDescending(g => g.Period)
                .ToListAsync();
            return Page();
        }
    }
}