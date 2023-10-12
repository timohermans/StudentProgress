using System;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using StudentProgress.Web.Lib.Data;
using StudentProgress.Web.Lib.Extensions;
using StudentProgress.Web.Models;

namespace StudentProgress.Web.Pages.StudentGroups
{
    public class CreateModel : PageModel
    {
        private readonly WebContext _db;

        [BindProperty] public Adventure Adventure { get; set; } = default!;

        public CreateModel(WebContext db)
        {
            _db = db;
        }

        public IActionResult OnGet()
        {
            Adventure = new()
            {
                DateStart = DateTime.Today,
                Name = ""
            };

            return this.PageOrPartial("_CreateForm");
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                return this.PageOrPartial("_CreateForm");
            }

            await _db.Adventures.AddAsync(Adventure, token);
            await _db.SaveChangesAsync(token);
            
            return RedirectToPage("./Index");
        }
    }
}