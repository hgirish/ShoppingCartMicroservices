using System;
using System.Collections.Generic;
using System.Linq;

namespace ShoppingCart.EventFeed
{
    public class EventStroe : IEventStore
    {
        private static readonly List<Event> database = new();
        public IEnumerable<Event> GetEvents(long firstEventSequenceNumber, long lastEventSequenceNumber)
        {
            return database.Where(e => e.SequenceNumber >= firstEventSequenceNumber && e.SequenceNumber <= lastEventSequenceNumber)
                .OrderBy(e => e.SequenceNumber);
        }

        public void Raise(string eventName, object content)
        {
            var seqNumber = database.NextSequenceNumber();
            var newEvent  = new Event(seqNumber, DateTimeOffset.UtcNow, eventName, content);
            database.Add(newEvent);
        }
    }

    public static class DatabaseExtension
    {
        public static long NextSequenceNumber(this List<Event> database)
        {
            return database.Any() ? database.Max(d => d.SequenceNumber) + 1 : 1;
        }
    }
}
