using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventCatalogAPI.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EventCatalogAPI
{
    public class Program
    {
        // Entry point
        public static void Main(string[] args)
        {
            // Building VM
            var host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope()) // asking all the dependancies
            {
                var servieProviders = scope.ServiceProvider;
                var context = servieProviders.GetRequiredService<CatalogContext>();
                CatalogSeed.Seed(context);
            }
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
