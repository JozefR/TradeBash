using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TradeBash.Core.Entities.Warehouse;
using TradeBash.Infrastructure.Data.Repositories;
using TradeBash.Infrastructure.DTO;
using TradeBash.Infrastructure.Services;

namespace TradeBash.DataCentre
{
    public class DataPopulator : BackgroundService
    {
        private readonly ILogger<DataPopulator> _logger;
        private readonly IServiceProvider _services;
        private readonly string _iexPath;

        private IApiClient _apiClient;
        private IStockRepository _stockRepository;
        private IStocksCsvReader _stocksCsvReader;

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

        private async Task DataPopulatorEngine()
        {
            using (var scope = _services.CreateScope())
            {
                _stocksCsvReader = scope.ServiceProvider.GetRequiredService<IStocksCsvReader>();
                _stockRepository = scope.ServiceProvider.GetRequiredService<IStockRepository>();
                _apiClient = scope.ServiceProvider.GetRequiredService<IApiClient>();

                var stocksToUpdate = _stocksCsvReader.LoadFile(IndexVersion.Spy100);

                foreach (var (symbol, name) in stocksToUpdate)
                {
                    var existingStock = await _stockRepository.GetBySymbolAsync(symbol);

                    if (existingStock == null)
                    {
                        var stocksHistorySerialized = await GetSerializedStocksFromDataProviderAsync(symbol, "max");
                        await AddHistoryToStockAsync(symbol, name, stocksHistorySerialized);
                    }
                    else
                    {
                        var lastDateDifference = GetDateDifferenceFromStockLastDate(existingStock);
                        var range = GetRangeForHistoricalData(lastDateDifference);
                        var stocksHistorySerialized = await GetSerializedStocksFromDataProviderAsync(symbol, range);
                        RemoveExistingHistory(existingStock, stocksHistorySerialized);
                        await AddHistoryToStockAsync(existingStock, stocksHistorySerialized);
                    }
                }
            }
        }

        private async Task<List<StockDtoResponse>> GetSerializedStocksFromDataProviderAsync(string symbol, string range)
        {
            var constructedPath = string.Format(_iexPath, symbol, range);
            var stocksHistory = await _apiClient.GetStocksAsync(constructedPath);
            var stocksHistorySerialized = stocksHistory.Select(JsonExtensions.MapDataResponse);
            return stocksHistorySerialized.ToList();
        }

        private async Task AddHistoryToStockAsync(string symbol, string name, IEnumerable<StockDtoResponse> stocksHistorySerialized)
        {
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

            await _stockRepository.AddAsync(stock);
        }

        private async Task AddHistoryToStockAsync(Stock existingStock, IEnumerable<StockDtoResponse> stocksHistorySerialized)
        {
            foreach (var stockResponse in stocksHistorySerialized)
            {
                existingStock.AddHistory(
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

            await _stockRepository.UpdateAsync(existingStock);
        }

        private TimeSpan GetDateDifferenceFromStockLastDate(Stock existingStock)
        {
            var existingStockLastDate = existingStock.History.Max(x => x.Date);
            var lastDateDifference = DateTime.Now - existingStockLastDate;
            return lastDateDifference;
        }

        private void RemoveExistingHistory(Stock existingStock, List<StockDtoResponse> stocksHistorySerialized)
        {
            var lastStock = existingStock.History.Max(x => x.Date);

            while (stocksHistorySerialized.Count != 0)
            {
                var stock = stocksHistorySerialized[0];

                if (stock.Date > lastStock.Date)
                {
                    break;
                }

                stocksHistorySerialized.Remove(stock);
            }
        }

        private string GetRangeForHistoricalData(TimeSpan lastDateDifference)
        {
            if (lastDateDifference < TimeSpan.FromDays(5))
            {
                return "5d";
            }
            if (lastDateDifference < TimeSpan.FromDays(30))
            {
                return "1m";
            }
            if (lastDateDifference < TimeSpan.FromDays(90))
            {
                return "3m";
            }
            if (lastDateDifference < TimeSpan.FromDays(180))
            {
                return "6m";
            }
            if (lastDateDifference < TimeSpan.FromDays(360))
            {
                return "1y";
            }
            if (lastDateDifference < TimeSpan.FromDays(720))
            {
                return "2y";
            }
            if (lastDateDifference < TimeSpan.FromDays(1800))
            {
                return "5y";
            }

            return "max";
        }
    }
}