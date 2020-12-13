using CSharpFunctionalExtensions;
using NHibernate.Linq;
using StudentProgress.Application.Groups;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
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
            var group = await _unitOfWork.Query<Group>().FirstOrDefaultAsync(g => g.Id == request.GroupId);

            if (group == null)
            {
                return Result.Failure("Group doesn't exist");
            }

            var student = await _unitOfWork.Query<Student>().FirstOrDefaultAsync(s => s.Name == request.Name);

            if (student == null)
            {
                student = new Student(request.Name);
                await _unitOfWork.SaveOrUpdateAsync(student);
            }

            var result = group.AddStudent(student);

            if (result.IsSuccess)
            {
                await _unitOfWork.CommitAsync();
                return Result.Success();
            }
            else
            {
                return result;
            }

        }
    }
}
