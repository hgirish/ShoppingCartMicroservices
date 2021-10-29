using EventStore.ClientAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ShoppingCart.EventFeed
{
    public class EsEventStore : IEventStore
    {
        private const string _connectionString = "tcp://admin:changeit@localhost:1113";
        private const string StreamName = "ShoppingCart";

        public async Task<IEnumerable<Event>> GetEventsAsync(long firstEventSequenceNumber, long lastEventSequenceNumber)
        {
            using var connection = EventStoreConnection.Create(
                connectionSettings: ConnectionSettings.Create().DisableTls().Build(),
                uri: new Uri(_connectionString));
            await connection.ConnectAsync();

            var result = await connection.ReadStreamEventsForwardAsync(
                stream: StreamName,
                start: firstEventSequenceNumber,
                count: (int)(lastEventSequenceNumber - firstEventSequenceNumber),
                resolveLinkTos: false);
            return result.Events
                .Select(e => new
                {
                    Content = Encoding.UTF8.GetString(e.Event.Data),
                    MetaData = JsonSerializer.Deserialize<EventMetaData>(
                        Encoding.UTF8.GetString(e.Event.Metadata),
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        })
                })
                .Select((e, i) => new Event(
                    SequenceNumber: i + firstEventSequenceNumber,
                    OccuredAt: e.MetaData.OccurredAt,
                    Name: e.MetaData.EventName,
                    Content: e.Content));

        }

        public async Task RaiseAsync(string eventName, object content)
        {
            using var connection = EventStoreConnection.Create(ConnectionSettings.Create().DisableTls().Build(), new Uri(_connectionString));
            await connection.ConnectAsync();
            await connection.AppendToStreamAsync(
                stream: StreamName,
                expectedVersion: ExpectedVersion.Any,
                events: new EventData(
                    eventId: Guid.NewGuid(),
                type: "ShoppingCartEvent",
                isJson: true,
                data: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(content)),
                metadata: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(
                    new EventMetaData(OccurredAt: DateTimeOffset.UtcNow, EventName: eventName)
                    )
                )));
        }
        public record EventMetaData(DateTimeOffset OccurredAt, string EventName);
    }
}
