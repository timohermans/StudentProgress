using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentProgress.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentProgress.Web.Pages.StudentGroups
{
    public class IndexModel : PageModel
    {
        private readonly ProgressContext _context;

        public IndexModel(ProgressContext context)
        {
            _context = context;
        }

        public IList<StudentGroup> StudentGroup { get; set; }

        public async Task OnGetAsync()
        {
            StudentGroup = await _context.StudentGroup.ToListAsync();
        }
    }
}
