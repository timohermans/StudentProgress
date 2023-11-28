using Microsoft.EntityFrameworkCore;
using StudentProgress.Core.Models;

namespace StudentProgress.Core.Data;

public class WebContext(DbContextOptions<WebContext> options) : DbContext(options)
{
    public DbSet<Adventure> Adventures => Set<Adventure>();
    public DbSet<Person> People => Set<Person>();
    public DbSet<QuestLine> QuestLines => Set<QuestLine>();
    public DbSet<Quest> Quests => Set<Quest>();
    public DbSet<Objective> Objectives => Set<Objective>();
}