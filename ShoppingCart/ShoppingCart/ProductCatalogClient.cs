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
        
        private readonly HttpClient _client;
        private readonly ICache _cache;
        private static string productCatalogBaseUrl = @"https://git.io/JeHiE";
        private static string getProductPathTemplate = "?productIds[{0}]";


        public ProductCatalogClient(HttpClient client, ICache cache)
        {
            client.BaseAddress = new Uri(productCatalogBaseUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client = client;
            _cache = cache;
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
            var response = _cache.Get(productsResource) as HttpResponseMessage;
            if (response is null)
            {
                response = await _client.GetAsync(productsResource);
                AddToCache(productsResource, response);
            }
            return response;
        }

        private void AddToCache(string productsResource, HttpResponseMessage response)
        {
            var cacheHeader = response.Headers.FirstOrDefault(h => h.Key == "cache-control");
            if (!string.IsNullOrEmpty(cacheHeader.Key) && 
                CacheControlHeaderValue.TryParse(cacheHeader.Value.ToString(), out var cacheControl) && 
                cacheControl.MaxAge.HasValue)
            {
                _cache.Add(productsResource, response, cacheControl.MaxAge.Value);
            }
        }

        private record ProductCatalogProduct(
       int ProductId, string ProductName, string ProductDescription, Money Price);
    }
}
