using Microsoft.EntityFrameworkCore;
using StudentProgress.Web.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace StudentProgress.Web.Lib.Data;

public class WebContext : IdentityDbContext<User>
{
    public DbSet<Adventure> Adventures => Set<Adventure>();
    public DbSet<Person> People => Set<Person>();

    public WebContext(DbContextOptions<WebContext> options) : base(options)
    {
    }
}