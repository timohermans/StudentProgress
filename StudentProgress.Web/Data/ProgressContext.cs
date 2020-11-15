using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StudentProgress.Web.Models;

namespace StudentProgress.Web.Data
{
    public class ProgressContext : DbContext
    {
        public ProgressContext (DbContextOptions<ProgressContext> options)
            : base(options)
        {
        }

        public DbSet<StudentProgress.Web.Models.StudentGroup> StudentGroup { get; set; }
    }
}
