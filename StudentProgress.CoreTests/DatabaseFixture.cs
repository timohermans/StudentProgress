using Microsoft.Extensions.Configuration;
using StudentProgress.Web.Lib.Data;

namespace StudentProgress.CoreTests
{
    public class DatabaseFixture
    {
        public string ConnectionString { get; private set; }

        public WebDataMother WebDataMother { get; }

        public DbContextOptions<WebContext> WebContextOptions { get; private set; }

        public DatabaseFixture()
        {
            ConnectionString = GetConnectionString();

            WebContextOptions = CreateWebContextOptions();
            WebDataMother = new WebDataMother(WebContextOptions);
            using var webContext = new WebContext(WebContextOptions);
            webContext.Database.EnsureDeleted();
            webContext.Database.Migrate();
        }

        private string GetConnectionString()
        {
            // so I'm too tired to figure this out,
            // but the integration tests look at the appsettings.Development.json
            // of the web project, but because (I think) the executing assembly is test
            // it tries to locate the web names in this project.
            // it's ugly, I should overwrite it in the integration setup...but I wont :')
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, false)
                .Build();
            var envCString = Environment.GetEnvironmentVariable("ConnectionStrings__ProgressContext");
            var cString = configuration.GetConnectionString("ProgressContext"); 

            return ConnectionString = envCString ?? cString ??
                throw new NullReferenceException(
                    "Connectionstring could not be found in either env var or appsettings");
        }

        private static DbContextOptions<WebContext> CreateWebContextOptions()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, false)
                .Build();
            var envCString = Environment.GetEnvironmentVariable("ConnectionStrings__WebContext");
            var cString = configuration.GetConnectionString("WebContext");

            var connectionString = envCString ?? cString ??
                throw new NullReferenceException(
                    "ConnectionString__WebContext could not be found in either env var or appsettings");

            return new DbContextOptionsBuilder<WebContext>()
                .UseSqlite(connectionString)
                .Options;
        }

        public WebContext CreateWebContext()
        {
            return new WebContext(WebContextOptions);
        }
    }
}