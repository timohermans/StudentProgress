using Microsoft.EntityFrameworkCore;
using StudentProgress.Web.Models;

namespace StudentProgress.Web.Lib.Data;

public class WebContext : DbContext
{
    public DbSet<Adventure> Adventures => Set<Adventure>();
    public DbSet<Person> People => Set<Person>();

    public WebContext(DbContextOptions<WebContext> options) : base(options)
    {
    }
}