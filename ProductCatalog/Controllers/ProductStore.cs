using System.Collections.Generic;
using System.Linq;

namespace ProductCatalog.Controllers
{
    public class ProductStore: IProductStore
    {
        public IEnumerable<ProductCatalogProduct> GetProductsByIds(IEnumerable<int> productIds) =>
            productIds.Select(id => new ProductCatalogProduct(id, "foo" + id, "bar", new Money()));

        
    }

}
