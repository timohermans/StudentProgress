using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using StudentProgress.Core.Entities;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> Get(SearchStudents.Query query)
        {
            return Ok(await _useCase.HandleAsync(query));
        }
    }
}