using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MinimalApi.Tests
{
    internal class MinimalApiTestHost : WebApplicationFactory<Program>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            // Add mock/test services to the builder here
            builder.ConfigureServices(services =>
            {
                services.AddScoped(sp =>
                {
                    // Replace SQLite with in-memory database for tests
                    return new DbContextOptionsBuilder<ProductContext>()
                    .UseInMemoryDatabase("Tests")
                    .UseApplicationServiceProvider(sp)
                    .Options;
                });
            });

            return base.CreateHost(builder);
        }
    }
}
