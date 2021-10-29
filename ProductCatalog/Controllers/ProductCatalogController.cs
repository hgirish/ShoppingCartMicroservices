using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ProductCatalog.Controllers
{
    [ApiController]
    [Route("/products")]
    public class ProductCatalogController : ControllerBase
    {
        private readonly IProductStore _productStore;

        public ProductCatalogController(IProductStore productStore)
        {
            _productStore = productStore;
        }

        [HttpGet("")]
        [ResponseCache(Duration =86400)]
        public IEnumerable<ProductCatalogProduct> Get([FromQuery] string productIds)
        {
            var products = _productStore.GetProductsByIds(ParseProductIdsFromQueryString(productIds));
            return products;
        }

        private static IEnumerable<int> ParseProductIdsFromQueryString(string productIdsString)
        {
            return productIdsString.Split(',')
                .Select(s => s.Replace("[", "").Replace("]", ""))
                .Select(int.Parse);
        }
    }

}
