using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using StudentProgress.Core.Entities;

namespace StudentProgress.Core.UseCases
{
    public class GroupUpdate
    {
        public class Request
        {
            public int Id { get; set; }
            [Required]
            public string Name { get; set; } = null!;
            public string? Mnemonic { get; set; }
        }

        private readonly ProgressContext _context;

        public GroupUpdate(ProgressContext context)
        {
            _context = context;
        }

        public async Task<Result> HandleAsync(Request request)
        {
            var studentGroup = Maybe<Group>.From(await _context.Groups.FindAsync(request.Id));

            return await studentGroup.ToResult("Group does not exist")
                .Check(group => group.UpdateGroup(Name.Create(request.Name).Value, request.Mnemonic))
                .Tap(() => _context.SaveChangesAsync());
            ;
        }
    }
}