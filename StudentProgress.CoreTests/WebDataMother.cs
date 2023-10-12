using StudentProgress.Web.Lib.Data;
using StudentProgress.Web.Models;

namespace StudentProgress.CoreTests;

public class WebDataMother
{
    private class WebContextAlwaysPersist : IDisposable
    {
        public WebContext Db { get; }

        public WebContextAlwaysPersist(DbContextOptions<WebContext> options)
        {
            Db = new WebContext(options);
        }

        public void Dispose()
        {
            Db.SaveChanges();
            Db.Dispose();
        }
    }

    private DbContextOptions<WebContext> ContextOptions { get; }

    public WebDataMother(DbContextOptions<WebContext> contextOptions)
    {
        ContextOptions = contextOptions;
    }

    public async Task<Adventure> CreateAdventure(Adventure adventure)
    {
        using var db = new WebContextAlwaysPersist(ContextOptions);
        await db.Db.Adventures.AddAsync(adventure);
        return adventure;
    }
}