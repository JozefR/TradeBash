using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TradeBash.Core.Entities.Strategy;
using TradeBash.Core.Entities.Warehouse;
using TradeBash.DataCentre;
using TradeBash.Infrastructure.Data.Repositories;
using TradeBash.Infrastructure.Services;
using TradeBash.SharedKernel.Interfaces;

namespace TradeBash.Web.Api
{
    public class StrategiesController : BaseApiController
    {
        private readonly ILogger<StrategiesController> _logger;
        private readonly IRepository _repository;
        private readonly IStrategyRepository _strategyRepository;
        private readonly IExcelReporting _excelReporting;
        private readonly IStocksCsvReader _csvReader;

        public StrategiesController(
            IRepository repository,
            IStrategyRepository strategyRepository,
            ILogger<StrategiesController> logger,
            IExcelReporting excelReporting,
            IStocksCsvReader csvReader)
        {
            _repository = repository;
            _strategyRepository = strategyRepository;
            _logger = logger;
            _excelReporting = excelReporting;
            _csvReader = csvReader;
        }

        [HttpGet("calculateStrategy/{sma}/{rsi}")]
        public async Task<IActionResult> calculateStrategy(int sma, int rsi)
        {
            var strategyName = $"SMA-{sma}-RSI-{rsi}";
            var strategy = Strategy.From(strategyName, 50000, sma, rsi);

            try
            {
                await CalculateAsync(strategy);

                return Ok();
            }
            catch (Exception e)
            {
                return UnprocessableEntity(e);
            }
        }

        [HttpGet("calculateStrategy/{smaShort}/{smaLong}/{rsi}")]
        public async Task<IActionResult> calculateStrategy(int smaShort, int smaLong, int rsi)
        {
            var strategyName = $"SMA-{smaShort}-SMA-{smaLong}-RSI-{rsi}";
            var strategy = Strategy.From(strategyName, 50000, smaShort, smaLong, rsi);

            try
            {
                await CalculateAsync(strategy);

                return Ok();
            }
            catch (Exception e)
            {
                return UnprocessableEntity(e);
            }
        }

        [HttpGet("calculateStrategy/{budget}/{smaShort}/{smaLong}/{rsi}")]
        public async Task<IActionResult> CalculateStrategy(int budget, int smaShort, int smaLong, int rsi)
        {
            var strategyName = $"Budget-{budget}-SMA-{smaShort}-SMA-{smaLong}-RSI-{rsi}";
            var strategy = Strategy.From(strategyName, budget, smaShort, smaLong, rsi);

            try
            {
                await CalculateAsync(strategy);
                return Ok();
            }
            catch (Exception e)
            {
                return UnprocessableEntity(e);
            }
        }

        public enum StrategyType
        {
            TestCase1,
            TestCase2,
            TestCase3,
            TestCase4
        }

        [HttpGet("Backtest/shortSmaRsi/{strategyType}/{strategyName}")]
        public async Task<List<GeneratedOrder>> Backtest(StrategyType strategyType, string strategyName)
        {
            var strategy = await _strategyRepository.GetByNameAsync(strategyName);

            _logger.LogInformation($"Started backtest for strategy {strategyName}");

            if (strategyType == StrategyType.TestCase1)
            {
                strategy.RunShortSmaRsi();
            }
            if (strategyType == StrategyType.TestCase2)
            {
                strategy.RunShortSmaLongSmaRsi(10, 20);
            }

            _logger.LogInformation($"Backtest for strategy {strategyName} finished");

            await _repository.UpdateAsync(strategy);

            _logger.LogInformation($"Backtest for strategy {strategyName} saved");
            
            return strategy.GeneratedOrders.ToList();
        }

        [HttpGet("ExportToExcel")]
        public async Task<OkResult> ExportToExcel()
        {
            var strategies = await _strategyRepository.GetAllGeneratedOrdersAsync();

            foreach (var strategy in strategies)
            {
                await _excelReporting.GenerateAsync(strategy);
            }

            return Ok();
        }

        [HttpGet("InMemoryBacktestWithExport/{strategyType}/{budget}/{smaShortParameter}/{smaLongParameter}/{rsiParameter}")]
        public async Task<IActionResult> InMemoryBacktestWithExport(
            StrategyType strategyType,
            IndexVersion indexVersion,
            int budget,
            int smaShortParameter,
            int smaLongParameter,
            int rsiParameter,
            int rsiValue,
            int allowedSlots)
        {
            try
            {
                var strategyName = $"{strategyType.ToString()}" +
                                   $"-{indexVersion.ToString()}" +
                                   $"-Budget-{budget}" +
                                   $"-SMAShort-{smaShortParameter}" +
                                   $"-SMALong-{smaLongParameter}" +
                                   $"-RSI-{rsiParameter}" +
                                   $"-RSIValue-{rsiValue}" +
                                   $"-AllowedSlots-{allowedSlots}";

                var strategy = Strategy.From(strategyName, budget, smaShortParameter, smaLongParameter, rsiParameter);

                var stocks = await _repository.ListNoTrackingAsync<Stock>();
                var stocksToCalculate = _csvReader.LoadFile(indexVersion);
                foreach (var (symbol, _) in stocksToCalculate)
                {
                    var stock = stocks.FirstOrDefault(x => x.Symbol == symbol);

                    if (stock != null)
                    {
                        _logger.LogInformation($"Start indicators calculation for stock {stock.Name}");

                        strategy.RunCalculationFor(stock);

                        _logger.LogInformation($"calculation for stock {stock.Name} finished");
                    }
                }

                _logger.LogInformation($"Started in memory backtest for strategy {strategyName}");

                if (strategyType == StrategyType.TestCase1)
                {
                    strategy.RunShortSmaRsi();
                }
                if (strategyType == StrategyType.TestCase2)
                {
                    strategy.RunShortSmaLongSmaRsi(rsiValue, 20);
                }
                if (strategyType == StrategyType.TestCase3)
                {
                    strategy.RunShortSmaLongSmaRsi(rsiValue, allowedSlots);
                }
                if (strategyType == StrategyType.TestCase3)
                {
                    strategy.RunShortSmaLongSmaRsi(rsiValue, allowedSlots);
                }
                if (strategyType == StrategyType.TestCase4)
                {
                    strategy.RunShortSmaLongSmaRsiWithSlots(rsiValue, allowedSlots);
                }

                _logger.LogInformation($"Started export to excel for strategy {strategyName}");

                await _excelReporting.GenerateAsync(strategy);

                _logger.LogInformation($"Excel report finished");

                return Ok();
            }
            catch (Exception e)
            {
                return UnprocessableEntity(e);
            }
        }

        private async Task CalculateAsync(Strategy strategy)
        {
            var stocks = await _repository.ListAsync<Stock>();

            await _repository.AddAsync(strategy);
            foreach (var stock in stocks)
            {
                _logger.LogInformation($"Start indicators calculation for stock {stock.Name}");

                strategy.RunCalculationFor(stock);

                _logger.LogInformation($"Saving indicator calculations to database");

                await _repository.UpdateAsync(strategy);
            }

            _logger.LogInformation("Saving Finished successfully");
        }
    }
}