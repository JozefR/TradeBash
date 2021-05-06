using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
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
                    var ws = package.Workbook.Worksheets.Add("MainReport");

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

                    var range = ws.Cells["A2"].LoadFromCollection(orders, true);
                    ws.Cells[1, 4, strategies.First().GeneratedOrders.Count + 2, 5].Style.Numberformat.Format = "dd-mm-yyyy";
                    range.AutoFitColumns();

                    // formats the header
                    ws.Cells["A1"].Value = $"{strategy.Name} Report";
                    ws.Cells["A1:C1"].Merge = true;

                    ws.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Row(1).Style.Font.Size = 24;

                    /*
                    ws.Row(1).Style.Font.Color.SetColor(Color.Coral);
                    */

                    ws.Row(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Row(2).Style.Font.Bold = true;
                    ws.Column(3).Width = 20;

                    // create chart
                    ExcelLineChart lineChart = ws.Drawings.AddChart("lineChart", eChartType.Line) as ExcelLineChart;

                    //set the title
                    lineChart.Title.Text = "LineChart Example";

                    //create the ranges for the chart
                    var rangeProfitLoss = ws.Cells[3,7,25,3];

                    lineChart.Series.Add(rangeProfitLoss);

                    //size of the chart
                    lineChart.SetSize(600, 300);

                    //add the chart at cell B6
                    lineChart.SetPosition(5, 0, 1, 0);

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