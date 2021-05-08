using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using TradeBash.Core.Entities.Strategy;
using TradeBash.Core.Entities.Warehouse;
using TradeBash.Infrastructure.Data.Repositories;
using TradeBash.SharedKernel.Interfaces;

namespace TradeBash.Web.Api
{
    public class StrategiesController : BaseApiController
    {
        private readonly ILogger<StrategiesController> _logger;
        private readonly IRepository _repository;
        private readonly IStrategyRepository _strategyRepository;

        public StrategiesController(
            IRepository repository,
            IStrategyRepository strategyRepository,
            ILogger<StrategiesController> logger)
        {
            _repository = repository;
            _strategyRepository = strategyRepository;
            _logger = logger;
        }

        [HttpGet("calculateStrategy/{sma}/{rsi}")]
        public async Task<IActionResult> calculateStrategy(int sma, int rsi)
        {
            var strategyName = $"SMA-{sma}-RSI-{rsi}";
            var strategy = Strategy.From(strategyName, sma, rsi);

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
            var strategy = Strategy.From(strategyName, smaShort, smaLong, rsi);

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
        public async Task<IActionResult> calculateStrategy(int budget, int smaShort, int smaLong, int rsi)
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

        [HttpGet("Backtest/{strategyName}")]
        public async Task<List<GeneratedOrder>> Backtest(string strategyName)
        {
            var strategy = await _strategyRepository.GetByNameAsync(strategyName);

            _logger.LogInformation($"Started backtest for strategy {strategyName}");
            
            strategy.RunBackTestForDate();

            _logger.LogInformation($"Backtest for strategy {strategyName} finished");

            await _repository.UpdateAsync(strategy);

            _logger.LogInformation($"Backtest for strategy {strategyName} saved");
            
            return strategy.GeneratedOrders.ToList();
        }

        public class DrawDown
        {
            public double Peak { get; set; }
            public double Trough { get; set; }
            public double MaxDrawDown { get; set; }

            public DrawDown()
            {
                Peak = 0;
                Trough = 0;
                MaxDrawDown = 0;
            }

            public void Calculate(double newValue)
            {
                if (newValue > Peak)
                {
                    Peak = newValue;
                    Trough = Peak;
                }
                else if (newValue < Trough)
                {
                    Trough = newValue;
                    var tmpDrawDown = Peak - Trough;
                    if (tmpDrawDown > MaxDrawDown)
                        MaxDrawDown = tmpDrawDown;
                }
            }
        }

        [HttpGet("ExportToExcel")]
        public async Task<OkResult> ExportToExcel()
        {
            var strategies = await _strategyRepository.GetAllGeneratedOrdersAsync();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            foreach (var strategy in strategies)
            {
                var file = new FileInfo($@"C:\Demos\{strategy.Name}.xlsx");

                if (file.Exists)
                {
                    file.Delete();
                }

                using (var package = new ExcelPackage(file))
                {
                    var strategyName = $"{strategy.Name} Report";
                    var ws = package.Workbook.Worksheets.Add(strategyName);

                    var nettProfit = strategy.GeneratedOrders.Sum(x => x.ProfitLoss);

                    var initialCapital = strategy.Budget;
                    var endingCapital = initialCapital + nettProfit;

                    var numberOfTrades = strategy.GeneratedOrders.Count;

                    var minDate = strategy.GeneratedOrders.Min(x => x.OpenDate);
                    var maxDate = strategy.GeneratedOrders.Max(x => x.CloseDate);
                    var testedHistory = $"{minDate.ToShortDateString()} - {maxDate.Value.ToShortDateString()}";

                    double winnerOrders = strategy.GeneratedOrders.Count(x => x.ProfitLoss > 0);
                    double allTrades = strategy.GeneratedOrders.Count;
                    double winnersPercentage = (winnerOrders / allTrades) * 100;

                    var drawDown = new DrawDown();
                    var ordered = strategy.GeneratedOrders.OrderBy(x => x.CloseDate);
                    foreach (var order in ordered)
                    {
                        drawDown.Calculate(order.ProfitLoss.Value);
                    }

                    // Results
                    // header
                    ws.Cells["A1"].Value = strategyName;
                    ws.Cells["A1:G1"].Merge = true;

                    ws.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Row(1).Style.Font.Size = 18;
                    // aggregates
                    ws.Cells["A2"].Value = "Initial Capital";
                    ws.Cells["B2"].Value = initialCapital;
                    ws.Row(2).Style.Font.Bold = true;

                    ws.Cells["A3"].Value = "Ending Capital";
                    ws.Cells["B3"].Value = endingCapital;
                    ws.Row(3).Style.Font.Bold = true;

                    ws.Cells["A4"].Value = "Total Net Profit";
                    ws.Cells["B4"].Value = nettProfit;
                    ws.Row(4).Style.Font.Bold = true;

                    ws.Cells["A5"].Value = "Total N. of Trade";
                    ws.Cells["B5"].Value = numberOfTrades;
                    ws.Row(5).Style.Font.Bold = true;

                    ws.Cells["A6"].Value = "Total Tested History";
                    ws.Cells["B6"].Value = testedHistory;
                    ws.Row(6).Style.Font.Bold = true;

                    ws.Cells["A7"].Value = "Percentage Winners";
                    ws.Cells["B7"].Value = winnersPercentage;
                    ws.Row(7).Style.Font.Bold = true;

                    ws.Cells["A8"].Value = "Profit Factor";
                    ws.Cells["B8"].Value = winnersPercentage;
                    ws.Row(8).Style.Font.Bold = true;

                    ws.Cells["A9"].Value = "Max. Drawdown";
                    ws.Cells["B9"].Value = drawDown.MaxDrawDown;
                    ws.Row(9).Style.Font.Bold = true;

                    // Data
                    var orders = strategy.GeneratedOrders.Select(x => new
                    {
                        x.Symbol,
                        x.OpenPrice,
                        x.ClosePrice,
                        x.OpenDate,
                        x.CloseDate,
                        x.BudgetInvestedPercentage,
                        x.ProfitLoss
                    }).OrderBy(x => x.OpenDate);

                    var range = ws.Cells["A11"].LoadFromCollection(orders, true);
                    ws.Cells[1, 4, strategies.First().GeneratedOrders.Count + 2, 5].Style.Numberformat.Format = "dd-mm-yyyy";
                    range.AutoFitColumns();

                    // format datetime
                    ws.Column(4).Style.Numberformat.Format = "dd-mm-yyyy";
                    ws.Column(5).Style.Numberformat.Format = "dd-mm-yyyy";

                    // formats the header
                    ws.Cells["A10"].Value = "Data";
                    ws.Cells["A10:G10"].Merge = true;
                    ws.Row(10).Style.Font.Size = 18;

                    /*
                    ws.Row(1).Style.Font.Color.SetColor(Color.Coral);
                    */

                    ws.Row(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Column(1).Width = 15;
                    ws.Column(2).Width = 15;
                    ws.Column(3).Width = 15;
                    ws.Column(4).Width = 15;
                    ws.Column(5).Width = 15;
                    ws.Column(6).Width = 15;
                    ws.Column(7).Width = 15;
                    ws.Column(1).AutoFit();
                    ws.Column(2).AutoFit();
                    ws.Column(3).AutoFit();
                    ws.Column(4).AutoFit();
                    ws.Column(5).AutoFit();
                    ws.Column(6).AutoFit();
                    ws.Column(7).AutoFit();

                    await package.SaveAsync();
                }
            }

            return Ok();
        }

        private async Task CalculateAsync(Strategy strategy)
        {
            var stocks = await _repository.ListAsync<Stock>();

            await _repository.AddAsync(strategy);

            foreach (var stock in stocks)
            {
                _logger.LogInformation($"Start indicators calculation for stock {stock.Name}");

                strategy.RunCalculationForStock(stock);

                _logger.LogInformation($"Saving indicator calculations to database");

                await _repository.UpdateAsync(strategy);
            }

            _logger.LogInformation("Saving Finished successfully");
        }
    }
}