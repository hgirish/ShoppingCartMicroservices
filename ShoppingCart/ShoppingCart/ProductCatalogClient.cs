using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace ShoppingCart.ShoppingCart
{
    public class ProductCatalogClient : IProductCatalogClient
    {

        private static readonly Dictionary<int, ShoppingCartItem> Database = new Dictionary<int, ShoppingCartItem>();
        private readonly HttpClient _client;
        private static string productCatalogBaseUrl = @"https://git.io/JeHiE";
        private static string getProductPathTemplate = "?productIds[{0}]";


        public ProductCatalogClient(HttpClient client)
        {
            client.BaseAddress = new Uri(productCatalogBaseUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client = client;
        }
        public async Task<IEnumerable<ShoppingCartItem>> GetShoppingCartItemsAsync(int[] productCatalogIds)
        {
            using var response = await RequestProductFromProductCatalog(productCatalogIds);
            return await ConvertToShoppingCartItems(response);
            
        }

        private static async Task<IEnumerable<ShoppingCartItem>> ConvertToShoppingCartItems(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();
            var products = await JsonSerializer.DeserializeAsync<List<ProductCatalogProduct>>(
                await response.Content.ReadAsStreamAsync(),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new();

            return products.Select(p => new ShoppingCartItem(p.ProductId, p.ProductName, p.ProductDescription, p.Price));
        }

        private async Task<HttpResponseMessage> RequestProductFromProductCatalog(int[] productCatalogIds)
        {
            var productsResource = string.Format(getProductPathTemplate, string.Join(",", productCatalogIds));
            return await _client.GetAsync(productsResource);
        }
        private record ProductCatalogProduct(
       int ProductId, string ProductName, string ProductDescription, Money Price);
    }
}
