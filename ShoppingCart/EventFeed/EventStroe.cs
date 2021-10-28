using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text.Json;
using System.Threading.Tasks;

namespace ShoppingCart.EventFeed
{
    public class EventStroe : IEventStore
    {
        
        private string _connectionString;

        public EventStroe(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConnectionStrings:ShoppingCart");
        }

        private const string _writeEventSql =
            @"Insert into EventStore(Name, OccurredAt, Content)
VALUES (@Name, @OccurredAt, @Content)";

        private const string _readEventsSql =
            @"Select * from EventStore where ID >= @Start and ID <= @End";

        

        public async Task<IEnumerable<Event>> GetEventsAsync(long firstEventSequenceNumber, long lastEventSequenceNumber)
        {
            await using var conn = new SqlConnection(_connectionString);
            return await conn.QueryAsync<Event>(
                _readEventsSql,
                new
                {
                    Start = firstEventSequenceNumber,
                    End = lastEventSequenceNumber
                });
        }

        public async Task RaiseAsync(string eventName, object content)
        {
            var jsonContent = JsonSerializer.Serialize(content);
            await using var conn = new SqlConnection(_connectionString);
            await conn.ExecuteAsync(
                _writeEventSql,
                new
                {
                    Name = eventName,
                    OccurredAt = DateTimeOffset.UtcNow,
                    Content = jsonContent
                });

            
        }
    }

  
}
