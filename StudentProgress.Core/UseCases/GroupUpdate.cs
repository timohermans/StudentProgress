using System.ComponentModel.DataAnnotations;
using System.Threading;
using CSharpFunctionalExtensions;
using StudentProgress.Core.Entities;

namespace StudentProgress.Core.UseCases
{
    public class GroupUpdate : IUseCaseBase<GroupUpdate.Request, Result>
    {
        public class Request : IUseCaseRequest<Result>
        {
            public int Id { get; set; }
            [Required] public string Name { get; set; } = null!;
            public DateTime StartDate { get; set; }
            public string? Mnemonic { get; set; }
        }

        private readonly ProgressContext _context;

        public GroupUpdate(ProgressContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(Request request, CancellationToken token)
        {
            var studentGroup = Maybe<StudentGroup?>.From(await _context.Groups.FindAsync(request.Id));
            var nameResult = Name.Create(request.Name);
            var periodResult = Period.Create(request.StartDate);
            var validationResult = Result.Combine(nameResult, periodResult);

            if (validationResult.IsFailure)
            {
                return validationResult;
            }
            

            return await studentGroup.ToResult("Group does not exist")
                .Check(group => group!.UpdateGroup(nameResult.Value, periodResult.Value, request.Mnemonic))
                .Tap(async () => await _context.SaveChangesAsync());
            ;
        }
    }
}