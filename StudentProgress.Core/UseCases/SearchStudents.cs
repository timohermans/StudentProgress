using StudentProgress.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace StudentProgress.Core.UseCases
{
    public class SearchStudents
    {
        public record Query
        {
            public string SearchTerm { get; set; } = null!;
        }

        public record GroupResponse
        {
            public int Id { get; }
            public string Name { get; }
        

            public GroupResponse(int id, string name)
            {
                Id = id;
                Name = name;
            }
        }

        public record Response
        {
            public int Id { get; set; }
            public string Name { get; set; } = null!;
            public string? AvatarPath { get; set; }
            public List<GroupResponse> Groups { get; set; } = new();
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
                        AvatarPath = s.AvatarPath,
                        Groups = s.StudentGroups
                            .Select(g => new GroupResponse(g.Id, g.Name))
                            .ToList()
                    })
                    .ToListAsync();
            }
        }
    }