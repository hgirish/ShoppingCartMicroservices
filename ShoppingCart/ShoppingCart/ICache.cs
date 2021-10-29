using System;

namespace ShoppingCart.ShoppingCart
{
    public interface ICache
    {
        void Add(string key, object value, TimeSpan ttl);
        object? Get(string key);
    }
}
