using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentProgress.Application.Groups.UseCases
{
    public class Update
    {
        public class Request
        {
            [Required]
            public int Id { get; set; }
            [Required]
            public string Name { get; set; } = null!;
            public string? Mnemonic { get; set; }
        }

        private readonly UnitOfWork _unitOfWork;

        public Update(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> HandleAsync(Request request)
        {
            return await _unitOfWork.GetAsync<Group>(request.Id)
                .ToResult($"Group {request.Id} doesn't exist")
                .Check(g => g.Update(request.Name, request.Mnemonic))
                .Tap(_ => _unitOfWork.CommitAsync())
                ;
        }
    }
}
