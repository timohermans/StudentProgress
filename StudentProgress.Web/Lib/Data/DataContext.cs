using Microsoft.EntityFrameworkCore;
using StudentProgress.Web.Models;

namespace StudentProgress.Web.Lib.Data;

public class DataContext : DbContext
{
    public required DbSet<Adventure> Adventures { get; set; }
    public required DbSet<Person> People { get; set; }

    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }
}