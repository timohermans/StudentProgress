using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentProgress.Web.Lib.Data;
using StudentProgress.Web.Models;

namespace StudentProgress.Web.Pages.People;

public class SearchModel : PageModel
{
    public List<Person> People { get; set; } = new();

    private readonly WebContext _db;

    public SearchModel(WebContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> OnGet(string q)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return Content("");
        }

        People = await _db.People
            .Include(p => p.Adventures)
            .Where(p => p.Name.ToLower().Contains(q.ToLower()))
            .OrderBy(p => p.Name)
            .Take(5)
            .Select(p => new Person
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Initials = p.Initials,
                AvatarPath = p.AvatarPath,
                Adventures = p.Adventures.Select(a => new Models.Adventure
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