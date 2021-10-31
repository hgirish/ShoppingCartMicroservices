using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Polly;
using ShoppingCart.EventFeed;
using ShoppingCart.ShoppingCart;
using System;

namespace ShoppingCart
{
    public class Startup
    {
        public Startup(IConfiguration config) => Configuration = config;

        private IConfiguration Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
           
            services.AddTransient<IShoppingCartStore, ShoppingCartStore>();
            services.AddTransient<IProductCatalogClient, ProductCatalogClient>();
            services.AddTransient<IEventStore, EventStroe>();
            services.AddTransient<ICache, Cache>();

            services.AddHttpClient<IProductCatalogClient, ProductCatalogClient>()
                .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, attempt => TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt))));

   

            services.AddHealthChecks()
                .AddCheck<ShoppingCartDbHealthCheck>(nameof(ShoppingCartDbHealthCheck),
                failureStatus: HealthStatus.Degraded,
                tags: new[] { "startup" }              
                )
                .AddCheck("LivenessHealthCheck",
                () => HealthCheckResult.Healthy(),
                tags: new[] { "liveness" });

            

            services.AddControllers();

        }

       
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseHealthChecks("/health/startup", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
            {
                Predicate = x => x.Tags.Contains("startup")
            });

            app.UseHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
            {
                Predicate = x => x.Tags.Contains("liveness")
            });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
