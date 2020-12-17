using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentProgress.Application.Groups.UseCases
{
    public class Create
    {
        private readonly UnitOfWork _unitOfWork;

        public Create(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public record Request
        {
            [Required]
            public string Name { get; init; } = null!;

            public string? Mnemonic { get; init; }
        }


        public async Task<Result<int>> HandleAsync(Request request)
        {
            if (_unitOfWork.Query<Group>().Any(g => g.Name == request.Name))
            {
                return Result.Failure<int>("Group already exists");
            }

            var group = new Group(GroupName.Create(request.Name).Value, request.Mnemonic);
            await _unitOfWork.SaveOrUpdateAsync(group);
            await _unitOfWork.CommitAsync();
            return Result.Success(group.Id);
        }
    }
}
