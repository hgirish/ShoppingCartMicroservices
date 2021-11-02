﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApiGateway.ProductList
{
    public class ProductListController : Controller
    {
        private readonly HttpClient _productCatalogClient;
        private readonly HttpClient _shoppingCartClient;

        public ProductListController(IHttpClientFactory httpClientFactory)
        {
            _productCatalogClient = httpClientFactory.CreateClient("ProductCatalogClient");
            _shoppingCartClient = httpClientFactory.CreateClient("ShoppingCartClient");
        }
        [HttpGet("/productlist")]
        public async Task<IActionResult> ProductList([FromQuery] int userId)
        {
            var products = await GetProductsFromCatalog();
            var cartProducts = await GetProductsFromCart(userId);
            return View(new ProductListViewModel(products,cartProducts));
        }

        [HttpPost("/shoppingcart/{userId}")]
        public async Task<OkResult> AddToCart(int userId, [FromBody] int productId)
        {
            var response = await _shoppingCartClient.PostAsJsonAsync($"/shoppingcart/{userId}/items", new[] { productId });
            return Ok();
        }

        [HttpDelete("/shoppingcart/{userId}")]
        public async Task<OkResult> RemoveFromCart(
            int userId,
            [FromBody] int productId)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"/shoppingcart/{userId}/items");
            request.Content = new StringContent(JsonSerializer.Serialize(new[] { productId }));
            var response = await _shoppingCartClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return Ok();
        }
        private async Task<Product[]> GetProductsFromCart(int userId)
        {
            var response = await _shoppingCartClient.GetAsync($"/shoppingcart/{userId}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStreamAsync();
            var cart = await JsonSerializer.DeserializeAsync<ShoppingCart>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return cart.Items;

        }

        private async Task<Product[]> GetProductsFromCatalog()
        {
            var response = await _productCatalogClient.GetAsync("/products?productIds=1,2,3,4");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStreamAsync();
            var products = await JsonSerializer.DeserializeAsync<Product[]>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return products;
        }
    }
    public record Product(int ProductId, string ProductName, string Description);
    public record ProductListViewModel(Product[] Products, Product[] CartProducts);
    public record ShoppingCart(int UserId, Product[] Items);
}
