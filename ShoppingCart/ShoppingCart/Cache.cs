using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.ShoppingCart
{
    public class Cache : ICache
    {
        private static readonly IDictionary<string, (DateTimeOffset, object)> cache =
            new ConcurrentDictionary<string, (DateTimeOffset, object)>();

        public void Add(string key, object value, TimeSpan ttl)
        {
            cache[key] = (DateTimeOffset.UtcNow.Add(ttl), value);
        }

        public object? Get(string key)
        {
            if (cache.TryGetValue(key, out var value) && value.Item1 > DateTimeOffset.UtcNow)
            {
                return value;
            }
            cache.Remove(key);
            return null;
        }
    }
}
