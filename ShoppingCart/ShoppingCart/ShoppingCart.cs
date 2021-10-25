using ShoppingCart.EventFeed;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShoppingCart.ShoppingCart
{
    public class ShoppingCart
    {
        private readonly HashSet<ShoppingCartItem> items = new();

        public int UserId { get; }

        public IEnumerable<ShoppingCartItem> Items => items;

        public ShoppingCart(int userId) => UserId = userId;

        public void AddItems(IEnumerable<ShoppingCartItem> shoppingCartItems, IEventStore eventStore)
        {
            foreach (var item in shoppingCartItems)
            {
               if( items.Add(item))
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
            var itemsRemoved =    items.RemoveWhere(i => productCatalogIds.Contains(i.ProductCatalogId));
            if (itemsRemoved > 0)
            {
                eventStore.Raise("ShoppingCartItemsDeleted", new { UserId, productCatalogIds });
            }
        }
    }
}
