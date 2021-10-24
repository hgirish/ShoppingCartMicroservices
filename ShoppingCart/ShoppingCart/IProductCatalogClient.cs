using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.ShoppingCart
{
    public interface IProductCatalogClient
    {
        IEnumerable<ShoppingCartItem> GetShoppingCartItems(int[] productIds);
    }
}
