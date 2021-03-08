using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentProgress.Api.ViewModels;
using StudentProgress.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentProgress.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly ProgressContext _context;

        public GroupController(ProgressContext context)
        {
            _context = context;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> Get()
        {
            return Ok(await _context.Groups.Select(g => new GroupViewModel
            {
                Id = g.Id,
                Name = g.Name.Value,
                Start = g.Period.StartDate,
                Mnemonic = g.Mnemonic
            }).ToListAsync());
        }
    }
}
