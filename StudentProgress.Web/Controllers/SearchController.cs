using System.Threading;
using Microsoft.AspNetCore.Mvc;
using StudentProgress.Core.Entities;
using System.Threading.Tasks;
using StudentProgress.Core.UseCases;

namespace StudentProgress.Web.Controllers
{
    [Route("api/search")]
    public class Search : Controller
    {
        private readonly SearchStudents _useCase;

        public Search(ProgressContext context)
        {
            _useCase = new SearchStudents(context);
        }

        [HttpGet("{searchTerm}")]
        public async Task<IActionResult> Get(SearchStudents.Query query, CancellationToken token)
        {
            return Ok(await _useCase.Handle(query, token));
        }
    }
}