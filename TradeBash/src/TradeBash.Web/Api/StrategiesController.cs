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
        private readonly IStockRepository _stockRepository;
        private readonly IExcelReporting _excelReporting;
        private readonly IStocksCsvReader _csvReader;

        public StrategiesController(
            IRepository repository,
            IStrategyRepository strategyRepository,
            ILogger<StrategiesController> logger,
            IExcelReporting excelReporting,
            IStocksCsvReader csvReader,
            IStockRepository stockRepository)
        {
            _repository = repository;
            _strategyRepository = strategyRepository;
            _logger = logger;
            _excelReporting = excelReporting;
            _csvReader = csvReader;
            _stockRepository = stockRepository;
        }

        [HttpGet("calculateStrategy/{indexVersion}/{budget}/{smaShort}/{smaLong}/{rsi}")]
        public async Task<IActionResult> CalculateStrategy(IndexVersion indexVersion, int budget, int smaShort, int smaLong, int rsi)
        {
            var strategyName = $"Budget-{budget}-SMA-{smaShort}-SMA-{smaLong}-RSI-{rsi}";
            var strategy = Strategy.From(strategyName, budget, smaShort, smaLong, rsi);

            try
            {
                await CalculateAsync(strategy, indexVersion);
                return Ok();
            }
            catch (Exception e)
            {
                return UnprocessableEntity(e);
            }
        }

        [HttpGet("Backtest/{strategyType}/{strategyName}")]
        public async Task<List<GeneratedOrder>> Backtest(StrategyType strategyType, string strategyName, int rsiValue, int allowedSlots)
        {
            var strategy = await _strategyRepository.GetByNameAsync(strategyName);

            _logger.LogInformation($"Started backtest for strategy {strategyName}");

            if (strategyType == StrategyType.TestCase1)
            {
                strategy.RunTestCase1(10);
            }
            if (strategyType == StrategyType.TestCase2)
            {
                strategy.RunTestCase2(rsiValue, allowedSlots);
            }
            if (strategyType == StrategyType.TestCase3)
            {
                strategy.RunTestCase3(rsiValue, allowedSlots);
            }
            if (strategyType == StrategyType.TestCase4)
            {
                strategy.RunTestCase4(rsiValue, allowedSlots);
            }

            _logger.LogInformation($"Backtest for strategy {strategyName} finished");

            await _repository.UpdateAsync(strategy);

            _logger.LogInformation($"Backtest for strategy {strategyName} saved");
            
            return strategy.GeneratedOrders.ToList();
        }

        private async Task CalculateAsync(Strategy strategy, IndexVersion indexVersion)
        {
            await _repository.AddAsync(strategy);

            var stocksToCalculate = _csvReader.LoadFile(indexVersion);

            foreach (var (symbol, _) in stocksToCalculate)
            {
                var stock = await _stockRepository.GetBySymbolAsync(symbol);

                if (stock != null)
                {
                    _logger.LogInformation($"Start indicators calculation for stock {stock.Name}");

                    strategy.RunCalculationFor(stock);

                    _logger.LogInformation($"calculation for stock {stock.Name} finished");
                }
            }

            await _repository.UpdateAsync(strategy);

            _logger.LogInformation("Saving Finished successfully");
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

        [HttpGet("InMemoryBacktestWithExport/TestCase1/")]
        public async Task<IActionResult> InMemoryBacktestWithExportTestCase1(
            IndexVersion indexVersion,
            int budget,
            int smaShortParameter,
            int rsiParameter,
            int rsiValue)
        {
            try
            {
                var strategyName = $"{StrategyType.TestCase1.ToString()}" +
                                   $"-{indexVersion.ToString()}" +
                                   $"-Budget-{budget}" +
                                   $"-SMAShort-{smaShortParameter}" +
                                   $"-RSI-{rsiParameter}" +
                                   $"-RSIValue-{rsiValue}";

                var strategy = Strategy.From(strategyName, budget, smaShortParameter, rsiParameter);

                var stocksToCalculate = _csvReader.LoadFile(indexVersion);
                foreach (var (symbol, _) in stocksToCalculate)
                {
                    var stock = await _stockRepository.GetBySymbolAsync(symbol);

                    if (stock != null)
                    {
                        _logger.LogInformation($"Start indicators calculation for stock {stock.Name}");

                        strategy.RunCalculationFor(stock);

                        _logger.LogInformation($"calculation for stock {stock.Name} finished");
                    }
                }

                _logger.LogInformation($"Started in memory backtest for strategy {strategyName}");

                strategy.RunTestCase1(rsiValue);

                _logger.LogInformation($"Started export to excel for strategy {strategyName}");

                if (!strategy.GeneratedOrders.Any())
                {
                    return Ok("No generated orders.");
                }

                await _excelReporting.GenerateAsync(strategy);

                _logger.LogInformation($"Excel report finished");

                return Ok();
            }
            catch (Exception e)
            {
                return UnprocessableEntity(e);
            }
        }

        [HttpGet("InMemoryBacktestWithExport/TestCase2/")]
        public async Task<IActionResult> InMemoryBacktestWithExportTestCase2(
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
                var strategyName = $"{StrategyType.TestCase2.ToString()}" +
                                   $"-{indexVersion.ToString()}" +
                                   $"-Budget-{budget}" +
                                   $"-SMAShort-{smaShortParameter}" +
                                   $"-SMAShort-{smaLongParameter}" +
                                   $"-RSI-{rsiParameter}" +
                                   $"-RSIValue-{rsiValue}";

                var strategy = Strategy.From(strategyName, budget, smaShortParameter, smaLongParameter, rsiParameter);

                var stocksToCalculate = _csvReader.LoadFile(indexVersion);
                foreach (var (symbol, _) in stocksToCalculate)
                {
                    var stock = await _stockRepository.GetBySymbolAsync(symbol);

                    if (stock != null)
                    {
                        _logger.LogInformation($"Start indicators calculation for stock {stock.Name}");

                        strategy.RunCalculationFor(stock);

                        _logger.LogInformation($"calculation for stock {stock.Name} finished");
                    }
                }

                _logger.LogInformation($"Started in memory backtest for strategy {strategyName}");

                strategy.RunTestCase2(rsiValue, allowedSlots);

                _logger.LogInformation($"Started export to excel for strategy {strategyName}");

                if (!strategy.GeneratedOrders.Any())
                {
                    return Ok("No generated orders.");
                }

                await _excelReporting.GenerateAsync(strategy);

                _logger.LogInformation($"Excel report finished");

                return Ok();
            }
            catch (Exception e)
            {
                return UnprocessableEntity(e);
            }
        }

        [HttpGet("InMemoryBacktestWithExport/TestCase3/")]
        public async Task<IActionResult> InMemoryBacktestWithExportTestCase3(
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
                var strategyName = $"{StrategyType.TestCase3.ToString()}" +
                                   $"-{indexVersion.ToString()}" +
                                   $"-Budget-{budget}" +
                                   $"-SMAShort-{smaShortParameter}" +
                                   $"-SMALong-{smaLongParameter}" +
                                   $"-RSI-{rsiParameter}" +
                                   $"-RSIValue-{rsiValue}";

                var strategy = Strategy.From(strategyName, budget, smaShortParameter, smaLongParameter, rsiParameter);

                var stocksToCalculate = _csvReader.LoadFile(indexVersion);
                foreach (var (symbol, _) in stocksToCalculate)
                {
                    var stock = await _stockRepository.GetBySymbolAsync(symbol);

                    if (stock != null)
                    {
                        _logger.LogInformation($"Start indicators calculation for stock {stock.Name}");

                        strategy.RunCalculationFor(stock);

                        _logger.LogInformation($"calculation for stock {stock.Name} finished");
                    }
                }

                _logger.LogInformation($"Started in memory backtest for strategy {strategyName}");

                strategy.RunTestCase3(rsiValue, allowedSlots);

                _logger.LogInformation($"Started export to excel for strategy {strategyName}");

                if (!strategy.GeneratedOrders.Any())
                {
                    return Ok("No generated orders.");
                }

                await _excelReporting.GenerateAsync(strategy);

                _logger.LogInformation($"Excel report finished");

                return Ok();
            }
            catch (Exception e)
            {
                return UnprocessableEntity(e);
            }
        }

        [HttpGet("InMemoryBacktestWithExport/TestCase4/")]
        public async Task<IActionResult> InMemoryBacktestWithExportTestCase4(
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
                var strategyName = $"{StrategyType.TestCase4.ToString()}" +
                                   $"-{indexVersion.ToString()}" +
                                   $"-Budget-{budget}" +
                                   $"-SMAShort-{smaShortParameter}" +
                                   $"-SMALong-{smaLongParameter}" +
                                   $"-RSI-{rsiParameter}" +
                                   $"-RSIValue-{rsiValue}";

                var strategy = Strategy.From(strategyName, budget, smaShortParameter, smaLongParameter, rsiParameter);

                var stocksToCalculate = _csvReader.LoadFile(indexVersion);
                foreach (var (symbol, _) in stocksToCalculate)
                {
                    var stock = await _stockRepository.GetBySymbolAsync(symbol);

                    if (stock != null)
                    {
                        _logger.LogInformation($"Start indicators calculation for stock {stock.Name}");

                        strategy.RunCalculationFor(stock);

                        _logger.LogInformation($"calculation for stock {stock.Name} finished");
                    }
                }

                _logger.LogInformation($"Started in memory backtest for strategy {strategyName}");

                strategy.RunTestCase4(rsiValue, allowedSlots);

                _logger.LogInformation($"Started export to excel for strategy {strategyName}");

                if (!strategy.GeneratedOrders.Any())
                {
                    return Ok("No generated orders.");
                }

                await _excelReporting.GenerateAsync(strategy);

                _logger.LogInformation($"Excel report finished");

                return Ok();
            }
            catch (Exception e)
            {
                return UnprocessableEntity(e);
            }
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

                var stocksToCalculate = _csvReader.LoadFile(indexVersion);
                foreach (var (symbol, _) in stocksToCalculate)
                {
                    var stock = await _stockRepository.GetBySymbolAsync(symbol);

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
                    strategy.RunTestCase1(rsiValue);
                }
                if (strategyType == StrategyType.TestCase2)
                {
                    strategy.RunTestCase2(rsiValue, allowedSlots);
                }
                if (strategyType == StrategyType.TestCase3)
                {
                    strategy.RunTestCase3(rsiValue, allowedSlots);
                }
                if (strategyType == StrategyType.TestCase4)
                {
                    strategy.RunTestCase4(rsiValue, allowedSlots);
                }

                _logger.LogInformation($"Started export to excel for strategy {strategyName}");

                if (!strategy.GeneratedOrders.Any())
                {
                    return Ok("No generated orders.");
                }

                await _excelReporting.GenerateAsync(strategy);

                _logger.LogInformation($"Excel report finished");

                return Ok();
            }
            catch (Exception e)
            {
                return UnprocessableEntity(e);
            }
        }
    }
}