using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.ShoppingCart
{
    [Route("/shoppingcart")]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IShoppingCartStore _shoppingCartStore;

        public ShoppingCartController(IShoppingCartStore shoppingCartStore)
        {
            _shoppingCartStore = shoppingCartStore;
        }
        [HttpGet("{userId:int}")]
        public ShoppingCart Get(int userId) => _shoppingCartStore.Get(userId);
    }
}
