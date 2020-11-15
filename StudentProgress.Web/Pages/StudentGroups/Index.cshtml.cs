using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentProgress.Web.Data;
using StudentProgress.Web.Models;

namespace StudentProgress.Web.Pages.StudentGroups
{
    public class IndexModel : PageModel
    {
        private readonly StudentProgress.Web.Data.ProgressContext _context;

        public IndexModel(StudentProgress.Web.Data.ProgressContext context)
        {
            _context = context;
        }

        public IList<StudentGroup> StudentGroup { get;set; }

        public async Task OnGetAsync()
        {
            StudentGroup = await _context.StudentGroup.ToListAsync();
        }
    }
}
