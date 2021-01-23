using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using StudentProgress.Core.Entities;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace StudentProgress.Web.Controllers
{
    [Route("api/search")]
    public class Search : Controller
    {
        public class GroupResponse
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class Response
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public List<GroupResponse> Groups { get; set; }
        }

        private readonly ProgressContext _context;

        public Search(ProgressContext context)
        {
            _context = context;
        }

        [HttpGet("{input}")]
        public async Task<IActionResult> Get(string input)
        {
            var students = await _context.Students
                .Include(s => s.StudentGroups)
                .Where(s => s.Name.ToLower().Contains(input))
                .OrderByDescending(s => s.ProgressUpdates.FirstOrDefault().Date)
                .Take(10)
                .Select(s => new Response
                {
                    Id = s.Id,
                    Name = s.Name,
                    Groups = s.StudentGroups.Select(g => new GroupResponse
                        {
                            Id = g.Id,
                            Name = g.Name
                        })
                        .ToList()
                })
                .ToListAsync();

            return Ok(students);
        }
    }
}