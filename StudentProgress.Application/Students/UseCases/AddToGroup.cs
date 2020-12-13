using CSharpFunctionalExtensions;
using NHibernate.Linq;
using StudentProgress.Application.Groups;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace StudentProgress.Application.Students.UseCases
{
    public class AddToGroup
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddToGroup(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public record Request
        {
            [Required]
            public string Name { get; init; } = null!;
            [Required]
            public int GroupId { get; init; }
        };

        public async Task<Result> HandleAsync(Request request)
        {
            var group = await _unitOfWork.GetAsync<Group>(request.GroupId);
            var student = Maybe<Student>.From(await _unitOfWork.Query<Student>().FirstOrDefaultAsync(s => s.Name == request.Name));

            return await group
                .ToResult($"Group {request.GroupId} doesn't exist")
                .Check(g => g.AddStudent(student.HasValue ? student.Value : new Student(request.Name)))
                .Tap(async r => await _unitOfWork.CommitAsync());
        }
    }
}
