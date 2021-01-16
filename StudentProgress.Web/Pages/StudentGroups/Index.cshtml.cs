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

        public IEnumerable<SelectListItem> Periods => _context.Groups
            .Select(g => g.Period)
            .Distinct()
            .Select(p => new SelectListItem(p.ToString(), p.StartDate.ToString("yyyy-M-d")))
            .ToList();

        public IndexModel(ProgressContext context)
        {
            _context = context;
        }

        public IList<StudentGroup> StudentGroup { get; set; }

        public async Task OnGetAsync()
        {
            StudentGroup = await _context.Groups.ToListAsync();
        }
    }
}
