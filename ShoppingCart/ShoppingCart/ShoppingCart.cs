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
                items.Add(item);
            }
        }

        public void RemoveItem(int[] productCatalogIds) => items.RemoveWhere(i => productCatalogIds.Contains(i.ProductCatalogId));

        
    }
}
