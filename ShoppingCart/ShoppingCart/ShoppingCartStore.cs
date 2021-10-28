using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using System.Linq;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ShoppingCart.ShoppingCart
{
    public class ShoppingCartStore : IShoppingCartStore
    {
        private static readonly Dictionary<int, ShoppingCart> Database = new Dictionary<int, ShoppingCart>();

        private string connectionString;

        private const string readItemSql = @"
Select ShoppingCart.ID, ProductCatalogId, 
ProductName, ProductDescription, Currency, Amount
From ShoppingCart, ShoppingCartItem
Where ShoppingCartItem.ShoppingCartId = ShoppingCart.Id
and ShoppingCart.UserId = @UserId
";

        private const string insertShoppingCartSql = @"Insert into ShoppingCart (UserId) output inserted.ID values (@UserId)";

        private const string deleteAllForShoppingCartSql = @"delete item from ShoppingCartItem item 
inner join ShoppingCart cart on item.shoppingCartId = cart.ID
and cart.UserId = @UserId";
        private const string addAllForShoppingCartSql = @"Insert into ShoppingCartItem
(ShoppingCartId, ProductCatalogId, ProductName, 
ProductDescription, Amount, Currency)
VALUES
(@ShoppingCartId, @ProductCatalogId, @ProductName, 
@ProductDescription, @Amount, @Currency)";


        public ShoppingCartStore(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("ConnectionStrings:ShoppingCart");
        }
       

        async Task<ShoppingCart> IShoppingCartStore.Get(int userId)
        {
            await using var conn = new SqlConnection(connectionString);
            var items = (await conn.QueryAsync(readItemSql, new { UserId = userId })).ToList();

            return new ShoppingCart(items.FirstOrDefault()?.ID, userId, items.Select(x => new ShoppingCartItem(
                (int)x.ProductCatalogId, x.ProductName, x.ProductDescription, new Money(x.Currency, x.Amount))));

        }

        async Task IShoppingCartStore.Save(ShoppingCart shoppingCart)
        {
            await using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();
            await using (var tx = conn.BeginTransaction())
            {
                var shoppingCartId = shoppingCart.Id ??
                    await conn.QuerySingleAsync<int>(insertShoppingCartSql, new { shoppingCart.UserId }, tx);

                await conn.ExecuteAsync(deleteAllForShoppingCartSql,(new { UserId = shoppingCart.UserId }, tx));

                await conn.ExecuteAsync(addAllForShoppingCartSql,
                    shoppingCart.Items.Select(x => new
                    {
                        shoppingCart,
                        x.ProductCatalogId,
                        ProductDescription = x.Description,
                        x.ProductName,
                        x.Price.Amount,
                        x.Price.Currency
                    }),tx);
                await tx.CommitAsync();
            }
        }
    }
}
