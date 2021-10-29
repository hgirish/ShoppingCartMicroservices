using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using ShoppingCart.EventFeed;
using ShoppingCart.ShoppingCart;
using System;

namespace ShoppingCart
{
    public class Startup
    {
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddTransient<IShoppingCartStore, ShoppingCartStore>();
            services.AddTransient<IProductCatalogClient, ProductCatalogClient>();
            services.AddTransient<IEventStore, EventStroe>();
            services.AddTransient<ICache, Cache>();

            services.AddHttpClient<IProductCatalogClient, ProductCatalogClient>()
                .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, attempt => TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt))));
           
        }

       
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
