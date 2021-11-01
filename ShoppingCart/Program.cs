using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Formatting.Json;

namespace ShoppingCart
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseSerilog((context,logger) =>
            {
                logger.Enrich.FromLogContext()
                .Enrich.WithSpan();
                if (context.HostingEnvironment.IsDevelopment())
                {
                    logger.WriteTo.ColoredConsole(outputTemplate: @"{Timestamp:yyyy-MM-dd HH:mm:ss} {TraceId} {Level:u3} {Message} {NewLine} {Exception}");
                }
                else
                {
                    logger.WriteTo.Console(new JsonFormatter());
                }
                
            })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
