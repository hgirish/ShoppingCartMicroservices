using System;
using System.Collections.Generic;

namespace ShoppingCart.ShoppingCart
{
    public class ProductCatalogClient : IProductCatalogClient
    {
        private static readonly Dictionary<int, ShoppingCartItem> Database = new Dictionary<int, ShoppingCartItem>();
        public IEnumerable<ShoppingCartItem> GetShoppingCartItems(int[] productIds)
        {
            var list = new List<ShoppingCartItem>();
            foreach (var productId in productIds)
            {
                list.Add(Database.ContainsKey(productId) ? Database[productId] : 
                    new ShoppingCartItem(productId, $"Item {productId}", $"Description {productId}", 
                    new Money("USD",
                   decimal.Round( Convert.ToDecimal( new Random().NextDouble() * 100),2, MidpointRounding.AwayFromZero)
                    )));
            }
            return list;
        }
    }
}
