using System;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentProgress.Web.Lib.Data;
using StudentProgress.Web.Lib.Extensions;
using StudentProgress.Web.Models;
using ICanvasApiConfig = StudentProgress.Web.Lib.CanvasApi.ICanvasApiConfig;

namespace StudentProgress.Web.Pages.StudentGroups
{
    public class IndexModel : PageModel
    {
        private readonly WebContext _context;
        private readonly ICanvasApiConfig _apiConfig;

        public bool CanImportGroups { get; private set; }
        public List<Adventure> Adventures { get; set; } = new();

        public IndexModel(WebContext context, ICanvasApiConfig apiConfig)
        {
            _context = context;
            _apiConfig = apiConfig;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            CanImportGroups = await _apiConfig.CanUseCanvasApiAsync();

            Adventures = await _context.Adventures
                .OrderByDescending(a => a.DateStart)
                .ToListAsync();

            if (Request.HasHtmxTrigger("adventures"))
            {
                return Partial("_List", Adventures);
            }

            if (Request.HasHtmxTrigger("cancel"))
            {
                return Partial("_Actions", CanImportGroups);
            }

            return Page();
        }

        public async Task<IActionResult> OnGetSingle(int id)
        {
            var adventure = await _context.Adventures.FindAsync(id);
            return Partial("_Row", adventure);
        }
    }
}