using ShoppingCart.EventFeed;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShoppingCart.ShoppingCart
{
    public class ShoppingCart
    {
        private readonly HashSet<ShoppingCartItem> _items = new();
        public int? Id { get; }
        public int UserId { get; }

        public IEnumerable<ShoppingCartItem> Items => _items;

        public ShoppingCart(int userId) => UserId = userId;
        public ShoppingCart(int? id, int userId, IEnumerable<ShoppingCartItem> items)
        {
            Id = id;
            UserId = userId;
         
            _items = new HashSet<ShoppingCartItem>(items);
        }

        public void AddItems(IEnumerable<ShoppingCartItem> shoppingCartItems, IEventStore eventStore)
        {
            foreach (var item in shoppingCartItems)
            {
               if( _items.Add(item))
                {
                    eventStore.Raise("ShoppingCartItemAdded", new { UserId, item });
                }
            }
        }

        public void RemoveItems(int[] productCatalogIds, IEventStore eventStore)
        {
            //foreach (var item in productCatalogIds)
            //{
            //    items.RemoveWhere( i => i.ProductCatalogId == item)
            //}
            var itemsRemoved =    _items.RemoveWhere(i => productCatalogIds.Contains(i.ProductCatalogId));
            if (itemsRemoved > 0)
            {
                eventStore.Raise("ShoppingCartItemsDeleted", new { UserId, productCatalogIds });
            }
        }
    }
}
