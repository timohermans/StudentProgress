using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StudentProgress.Core.Entities;
using StudentProgress.Core.UseCases;

namespace StudentProgress.Web.Controllers
{
    [Route("api/student")]
    public class StudentController : Controller
    {
        private readonly ProgressContext _context;
        private readonly StudentUpdate _useCase;

        public StudentController(ProgressContext context)
        {
            _context = context;
            _useCase = new StudentUpdate(context);
        }
        
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(StudentUpdate.Command command, CancellationToken token)
        {
            await _useCase.Handle(command, token);
            return Ok();
        }

        [HttpPut("{id:int}/name")]
        public async Task<IActionResult> UpdateName(StudentUpdate.Command command, CancellationToken token)
        {
            var student = await _context.Students.FindAsync(command.Id);
            command.Note = student?.Note;
            var result = await _useCase.Handle(command, token);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return Ok();
        }
    }
}