using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TradeBash.Core.Entities.Warehouse;
using TradeBash.Infrastructure.Data.Repositories;
using TradeBash.Infrastructure.Services;

namespace TradeBash.DataCentre
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureServices(services =>
                {
                    services.AddHostedService<ConsumeScopedServiceHostedService>();
                });
    }

    public class ConsumeScopedServiceHostedService : BackgroundService
    {
        private readonly ILogger<ConsumeScopedServiceHostedService> _logger;
        private readonly IServiceProvider _services;
        private readonly string _iexPath;

        public ConsumeScopedServiceHostedService(
            IServiceProvider services,
            IConfiguration configuration,
            ILogger<ConsumeScopedServiceHostedService> logger)
        {
            _services = services;
            _logger = logger;

            _iexPath = configuration.GetConnectionString("IEXConnection");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Consume Scoped Service Hosted Service running");

            await DoWork(stoppingToken);
        }

        // Todo: implement stock history actualization
        private async Task DoWork(CancellationToken stoppingToken)
        {
            int executionCount = 0;

            _logger.LogInformation("Consume Scoped Service Hosted Service is working");

            using (var scope = _services.CreateScope())
            {
                var csvReader = scope.ServiceProvider.GetRequiredService<IStocksCsvReader>();
                var repository = scope.ServiceProvider.GetRequiredService<IStockRepository>();
                var apiClient = scope.ServiceProvider.GetRequiredService<IApiClient>();

                var stocksToUpdate = csvReader.LoadFile(IndexVersion.Spy100);
                foreach (var toUpdate in stocksToUpdate)
                {
                    var existingStock = await repository.GetBySymbolAsync(toUpdate.symbol);
                    if (existingStock == null)
                    {
                        var iexPath = String.Format(_iexPath, String.Concat(toUpdate.symbol, String.Concat("1y")));
                        var stocksHistory = await apiClient.GetStocksAsync(iexPath);
                        var stocksHistorySerialized = stocksHistory.Select(x => x.MapDataResponse(toUpdate.symbol));

                        var stock = Stock.From(toUpdate.symbol, toUpdate.name);

                        foreach (var stockResponse in stocksHistorySerialized)
                        {
                            stock.AddHistory(
                                stockResponse.Date,
                                stockResponse.Open,
                                stockResponse.Close,
                                stockResponse.Label);
                        }

                        await repository.AddAsync(stock);
                    }
                }

                while (!stoppingToken.IsCancellationRequested)
                {
                    executionCount++;

                    _logger.LogInformation("Scoped Processing Service is working. Count: {Count}", executionCount);

                    await Task.Delay(10000, stoppingToken);
                }
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Consume Scoped Service Hosted Service is stopping");

            await base.StopAsync(stoppingToken);
        }
    }
}