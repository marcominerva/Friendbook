using Friendbook.DataAccessLayer;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Friendbook.Tests;

internal class FriendbookFactory : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        builder.ConfigureServices(services =>
        {
            var root = new InMemoryDatabaseRoot();
            services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));
            services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=TestServer;Integrated Security=true;")
                );
            //options.UseInMemoryDatabase("Testing", root));
        });

        return base.CreateHost(builder);
    }
}
