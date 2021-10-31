
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShoppingCart.EventFeed;
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
        private readonly IProductCatalogClient _productCatalogClient;
        private readonly IEventStore _eventStore;
        private readonly ILogger<ShoppingCartController> _logger;

        public ShoppingCartController(IShoppingCartStore shoppingCartStore,
            IProductCatalogClient productCatalogClient,
            IEventStore eventStore, 
            ILogger<ShoppingCartController> logger)
        {
            _shoppingCartStore = shoppingCartStore;
            _productCatalogClient = productCatalogClient;
            _eventStore = eventStore;
            _logger = logger;
        }
        [HttpGet("{userId:int}")]
        public async Task<ShoppingCart> GetAsync(int userId) =>await _shoppingCartStore.Get(userId);

        [HttpPost("{userId:int}/items")]
        public async Task<ShoppingCart> PostAsync(int userId, [FromBody] int[] productIds)
        {
            var shoppingCart = await _shoppingCartStore.Get(userId);
            var shoppingCartItems = await _productCatalogClient.GetShoppingCartItemsAsync(productIds);
            shoppingCart.AddItems(shoppingCartItems, _eventStore);
            await _shoppingCartStore.Save(shoppingCart);
            _logger.LogInformation("Successfully added products to shopping cart {@productIds}, {@shoppingCart}", productIds, shoppingCart);
            return shoppingCart;
        }

        [HttpDelete("{userId:int}/items")]
        public async Task<ShoppingCart> DeleteAsync(int userId, [FromBody] int[] productIds)
        {
            var shoppingCart = await  _shoppingCartStore.Get(userId);
            shoppingCart.RemoveItems(productIds, _eventStore);
            await _shoppingCartStore.Save(shoppingCart);
            return shoppingCart;
        }
    }
}
