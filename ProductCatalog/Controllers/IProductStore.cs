using System.Collections.Generic;

namespace ProductCatalog.Controllers
{
    public interface IProductStore
    {
        IEnumerable<ProductCatalogProduct> GetProductsByIds(IEnumerable<int> productIds);
    }

}
