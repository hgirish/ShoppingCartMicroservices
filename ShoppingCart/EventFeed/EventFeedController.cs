using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.EventFeed
{
    [Route("/events")]
    public class EventFeedController : ControllerBase
    {
        private readonly IEventStore _eventStore;

        public EventFeedController(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        [HttpGet("")]
        public async Task<Event[]> GetAsync(
            [FromQuery] long start,
            [FromQuery] long end = long.MaxValue) =>( await _eventStore.GetEventsAsync(start, end)).ToArray();
    }
}
