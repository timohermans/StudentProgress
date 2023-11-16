using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentProgress.Web.Lib.Data;
using StudentProgress.Web.Lib.Extensions;
using ICanvasApiConfig = StudentProgress.Web.Lib.CanvasApi.ICanvasApiConfig;

namespace StudentProgress.Web.Pages.Adventures
{
    public class IndexModel(WebContext context, ICanvasApiConfig apiConfig) : PageModel
    {
        public bool CanImportGroups { get; private set; }
        public List<Models.Adventure> Adventures { get; set; } = [];

        public async Task<IActionResult> OnGetAsync()
        {
            CanImportGroups = apiConfig.CanUseCanvasApiAsync();

            Adventures = await context.Adventures
                .Include(a => a.People)
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
            var adventure = await context.Adventures
                .Include(a => a.People)
                .FirstOrDefaultAsync(a => a.Id == id);
            return Partial("_Row", adventure);
        }

        public async Task<IActionResult> OnGetOptions(int id)
        {
             var adventure = await context.Adventures.FindAsync(id);
             return Partial("_Options", adventure);           
        }
    }
}