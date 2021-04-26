using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TradeBash.Infrastructure;
using TradeBash.Infrastructure.Data;
using TradeBash.Infrastructure.Data.Repositories;
using TradeBash.Infrastructure.Services;
using TradeBash.SharedKernel.Interfaces;

namespace TradeBash.DataCentre
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IRepository, EfRepository>();
            services.AddScoped<IStockRepository, StockRepository>();
            services.AddScoped<IStocksCsvReader, StocksCsvReader>();

            services.AddHttpClient<IApiClient, ApiClient>();

            var test = this.GetType().Assembly.FullName;
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context => { await context.Response.WriteAsync("Hello World!"); });
            });
        }
    }
}