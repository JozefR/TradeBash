using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TradeBash.Core.Entities.Warehouse;
using TradeBash.Infrastructure.Data.Repositories;
using TradeBash.Infrastructure.Services;

namespace TradeBash.DataCentre
{
    public class DataPopulator : BackgroundService
    {
        private readonly ILogger<DataPopulator> _logger;
        private readonly IServiceProvider _services;
        private readonly string _iexPath;

        public DataPopulator(
            IServiceProvider services,
            IConfiguration configuration,
            ILogger<DataPopulator> logger)
        {
            _services = services;
            _logger = logger;
            _iexPath = configuration.GetConnectionString("IEXConnection");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Consume Scoped Service Hosted Service running");

            await Start(stoppingToken);
        }

        private async Task Start(CancellationToken stoppingToken)
        {
            var executionCount = 0;

            _logger.LogInformation("Consume Scoped Service Hosted Service is working");

            while (!stoppingToken.IsCancellationRequested)
            {
                executionCount++;

                await DataPopulatorEngine();

                _logger.LogInformation("Scoped Processing Service is working. Count: {Count}", executionCount);

                await Task.Delay(10000, stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Consume Scoped Service Hosted Service is stopping");

            await base.StopAsync(stoppingToken);
        }

        // Todo: implement stock history actualization
        private async Task DataPopulatorEngine()
        {
            using (var scope = _services.CreateScope())
            {
                var csvReader = scope.ServiceProvider.GetRequiredService<IStocksCsvReader>();
                var repository = scope.ServiceProvider.GetRequiredService<IStockRepository>();
                var apiClient = scope.ServiceProvider.GetRequiredService<IApiClient>();

                var stocksToUpdate = csvReader.LoadFile(IndexVersion.Spy100);
                foreach (var (symbol, name) in stocksToUpdate)
                {
                    var existingStock = await repository.GetBySymbolAsync(symbol);

                    if (existingStock != null) continue;

                    var constructedPath = string.Format(_iexPath, symbol, "1m");
                    var stocksHistory = await apiClient.GetStocksAsync(constructedPath);
                    var stocksHistorySerialized = stocksHistory.Select(JsonExtensions.MapDataResponse);

                    var stock = Stock.From(symbol, name);

                    foreach (var stockResponse in stocksHistorySerialized)
                    {
                        stock.AddHistory(
                            stockResponse.Date,
                            stockResponse.Open,
                            stockResponse.Close,
                            stockResponse.Label,
                            stockResponse.High,
                            stockResponse.Low,
                            stockResponse.Volume,
                            stockResponse.ChangeOverTime,
                            stockResponse.MarketChangeOverTime,
                            stockResponse.UOpen,
                            stockResponse.UClose,
                            stockResponse.UHigh,
                            stockResponse.ULow,
                            stockResponse.UVolume,
                            stockResponse.FOpen,
                            stockResponse.FClose,
                            stockResponse.FHigh,
                            stockResponse.FLow,
                            stockResponse.FVolume,
                            stockResponse.Change,
                            stockResponse.ChangePercent);
                    }

                    await repository.AddAsync(stock);
                }
            }
        }
    }
}