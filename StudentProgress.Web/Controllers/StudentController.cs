using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StudentProgress.Core.Entities;
using StudentProgress.Core.UseCases;

namespace StudentProgress.Web.Controllers
{
    [Route("api/student")]
    public class StudentController : Controller
    {
        private readonly StudentUpdate _useCase;

        public StudentController(ProgressContext context)
        {
            _useCase = new StudentUpdate(context);
        }
        
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(StudentUpdate.Command command)
        {
            await _useCase.HandleAsync(command);
            return Ok();
        }
    }
}