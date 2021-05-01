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

        private IDataProvider _dataProvider;
        private IStockRepository _stockRepository;
        private IStocksCsvReader _stocksCsvReader;

        public DataPopulator(
            IServiceProvider services,
            ILogger<DataPopulator> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Hosted Service is working");

            while (!stoppingToken.IsCancellationRequested)
            {
                await DataPopulatorEngine();

                await Task.Delay(10000, stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Hosted Service is stopping");

            await base.StopAsync(stoppingToken);
        }

        private async Task DataPopulatorEngine()
        {
            using (var scope = _services.CreateScope())
            {
                _stocksCsvReader = scope.ServiceProvider.GetRequiredService<IStocksCsvReader>();
                _stockRepository = scope.ServiceProvider.GetRequiredService<IStockRepository>();
                _dataProvider = scope.ServiceProvider.GetRequiredService<IDataProvider>();

                var stocksToUpdate = _stocksCsvReader.LoadFile(IndexVersion.Spy100);

                foreach (var (symbol, name) in stocksToUpdate)
                {
                    var existingStock = await _stockRepository.GetBySymbolAsync(symbol);
                    if (existingStock == null)
                    {
                        var stocks = await _dataProvider.GetSerializedStocksFromDataProviderAsync(symbol, HistoryRange.Max);
                        await AddHistoryToDb(symbol, name, stocks);
                    }
                    else
                    {
                        var lastDateDifference = GetDateDifferenceFromStockLastDate(existingStock);
                        var historyRange = _dataProvider.GetRangeForHistoricalData(lastDateDifference);
                        var stocks = await _dataProvider.GetSerializedStocksFromDataProviderAsync(symbol, historyRange);
                        RemoveExistingHistory(existingStock, stocks);
                        await AddHistoryToDb(existingStock, stocks);
                    }
                }
            }
        }

        private async Task AddHistoryToDb(string symbol, string name, IEnumerable<StockDtoResponse> stocksHistorySerialized)
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

        private async Task AddHistoryToDb(Stock existingStock, IEnumerable<StockDtoResponse> stocksHistorySerialized)
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
    }
}