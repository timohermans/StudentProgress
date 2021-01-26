using System.Collections.Generic;
using System.Threading.Tasks;
using StudentProgress.Core.Entities;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace StudentProgress.Core.UseCases
{
    public class SearchStudents
    {
        public record Query
        {
            public string SearchTerm { get; set; }
        }

        public record GroupResponse
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public record Response
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public List<GroupResponse> Groups { get; set; }
        }

        private readonly ProgressContext _context;

        public SearchStudents(ProgressContext context)
        {
            _context = context;
        }

        public async Task<IList<Response>> HandleAsync(Query query)
        {
                return await _context.Students
                    .Include(s => s.StudentGroups)
                    .Where(s => s.Name.ToLower().Contains(query.SearchTerm))
                    .OrderByDescending(s => s.ProgressUpdates.FirstOrDefault()!.Date)
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
            }
        }
    }