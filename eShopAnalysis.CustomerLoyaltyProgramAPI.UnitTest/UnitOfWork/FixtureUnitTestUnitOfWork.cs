using eShopAnalysis.CustomerLoyaltyProgramAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.PostgreSql;

namespace eShopAnalysis.CustomerLoyaltyProgramAPI.UnitTest.Repository
{
    public class FixtureUnitTestUnitOfWork : IAsyncLifetime
    {
        private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder().WithImage("postgres:15-alpine")
            .Build();

        public string ConnectionString { get
            {
                return _postgreSqlContainer.GetConnectionString();
            } 
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
