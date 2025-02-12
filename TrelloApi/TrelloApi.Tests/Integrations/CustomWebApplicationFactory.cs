using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TrelloApi.app;

namespace TrelloApi.Tests.Integrations;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    private static string DatabaseName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptors = services.Where(
                d => d.ServiceType == typeof(DbContextOptions<TrelloContext>) ||
                     d.ServiceType == typeof(TrelloContext)
            ).ToList();

            foreach (var descriptor in descriptors)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<TrelloContext>(options =>
            {
                options.UseInMemoryDatabase(DatabaseName);
            });
            
            var sp = services.BuildServiceProvider();
            using (var scope = sp.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<TrelloContext>();
                db.Database.EnsureCreated();
            }
        });

        builder.UseEnvironment("Development");
    }
}