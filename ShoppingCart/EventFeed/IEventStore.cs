using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoppingCart.EventFeed
{
    public interface IEventStore
    {
        Task<IEnumerable<Event>> GetEventsAsync(long firstEventSequenceNumber, long lastEventSequenceNumber);
        Task RaiseAsync(string eventName, object content);
    }
}
