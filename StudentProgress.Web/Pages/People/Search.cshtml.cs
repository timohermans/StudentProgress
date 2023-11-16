using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentProgress.Core.Data;
using StudentProgress.Core.Models;

namespace StudentProgress.Web.Pages.People;

public class SearchModel(WebContext db) : PageModel
{
    public List<Person> People { get; set; } = [];

    public async Task<IActionResult> OnGet(string q)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return Content("");
        }

        People = await db.People
            .Include(p => p.Adventures)
            .Where(p => p.FirstName.ToLower().Contains(q.ToLower()))
            .OrderBy(p => p.FirstName)
            .Take(5)
            .Select(p => new Person
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Initials = p.Initials,
                AvatarPath = p.AvatarPath,
                Adventures = p.Adventures.Select(a => new Core.Models.Adventure
                {
                    Id = a.Id,
                    Name = a.Name,
                    DateStart = a.DateStart
                }).OrderByDescending(a => a.DateStart).ToList()
            })
            .ToListAsync();

        return Page();
    }
}