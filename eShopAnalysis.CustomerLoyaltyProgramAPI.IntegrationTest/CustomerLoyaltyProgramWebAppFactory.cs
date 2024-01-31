using eShopAnalysis.CustomerLoyaltyProgramAPI.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.PostgreSql;

namespace eShopAnalysis.CustomerLoyaltyProgramAPI.IntegrationTest
{
    public class CustomerLoyaltyProgramWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder().WithImage("postgres:15-alpine")
            .Build();

        public string ConnectionString
        {
            get {
                return _postgreSqlContainer.GetConnectionString();
            }
        }


        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.ConfigureServices(services =>
            {
                var descriptorType = typeof(DbContextOptions<PostgresDbContext>);
                var descriptor = services.SingleOrDefault(s => s.ServiceType == descriptorType);

                if (descriptor != null) {
                    services.Remove(descriptor);
                }

                services.AddDbContext<PostgresDbContext>(options => options.UseNpgsql(_postgreSqlContainer.GetConnectionString()));

            });
        }

        public Task DisposeAsync()
        {
            return _postgreSqlContainer.DisposeAsync().AsTask();
        }

        public Task InitializeAsync()
        {
            return _postgreSqlContainer.StartAsync();
        }
    }
}
