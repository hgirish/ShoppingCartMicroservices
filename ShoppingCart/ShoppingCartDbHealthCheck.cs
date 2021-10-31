using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingCart
{
    public class ShoppingCartDbHealthCheck : IHealthCheck
    {
        private readonly string _connectionString;

        public ShoppingCartDbHealthCheck(IConfiguration configuration)
        {
           // _connectionString = connectionString;
            
            _connectionString = configuration.GetConnectionString("ShoppingCart");
            System.Console.WriteLine($"connection string: {_connectionString}");
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            await using var conn =
                new SqlConnection(_connectionString);
            var result = await conn.QuerySingleAsync<int>("Select 1");
            return result == 1
                ? HealthCheckResult.Healthy()
                : HealthCheckResult.Degraded();
        }
    }
}
