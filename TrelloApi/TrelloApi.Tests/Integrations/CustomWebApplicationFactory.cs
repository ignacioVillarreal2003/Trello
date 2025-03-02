using System.Data.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TrelloApi.Application.Utils;
using TrelloApi.Infrastructure.Persistence.Data;

namespace TrelloApi.Tests.Integrations;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    private static readonly string InMemoryDatabaseName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptors = services.Where(
                d => d.ServiceType.Namespace?.StartsWith("Microsoft.EntityFrameworkCore") == true ||
                     d.ServiceType == typeof(DbContextOptions<TrelloContext>) ||
                     d.ServiceType == typeof(DbConnection)).ToList();

            foreach (var descriptor in descriptors)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<TrelloContext>(options =>
            {
                options.UseInMemoryDatabase(InMemoryDatabaseName);
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