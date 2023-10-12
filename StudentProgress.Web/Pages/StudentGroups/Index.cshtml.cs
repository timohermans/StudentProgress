using System;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

            return this.PageOrPartial("_IndexActions");
        }
    }
}