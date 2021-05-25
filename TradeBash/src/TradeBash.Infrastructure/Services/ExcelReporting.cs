using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using TradeBash.Core.Entities.Strategy;
using TradeBash.Core.Report;

namespace TradeBash.Infrastructure.Services
{
    public class ExcelReporting : IExcelReporting
    {
        private readonly IDrawdown _drawdown;

        public ExcelReporting(IDrawdown drawdown)
        {
            _drawdown = drawdown;
        }

        public async Task GenerateAsync(Strategy strategy)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var file = new FileInfo($@"C:\Demos\{strategy.Name}.xlsx");

            if (file.Exists)
            {
                file.Delete();
            }

            using (var package = new ExcelPackage(file))
            {
                var nettProfit = StrategyAggregates.GetNettProfit(strategy);
                var profitFactor = StrategyAggregates.GetProfitFactor(strategy);
                var endingCapital = StrategyAggregates.GetEndingCapital(strategy);
                var numberOfTrades = StrategyAggregates.GetNumberOfTrades(strategy);
                var testedHistory = StrategyAggregates.GetTestedHistory(strategy);
                var startDate = StrategyAggregates.GetStartDate(strategy);
                var endDate = StrategyAggregates.GetEndDate(strategy);
                var winnersPercentage = StrategyAggregates.GetPercentageWinners(strategy);

                _drawdown.Calculate(strategy);

                var strategyName = $"{strategy.Name}";
                var ws = package.Workbook.Worksheets.Add(strategyName);

                package.Workbook.Worksheets.Add("IgnoreErrors");
                // Results
                // header
                ws.Cells["A1"].Value = strategyName;
                ws.Cells["A1:G1"].Merge = true;

                ws.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Row(1).Style.Font.Size = 18;

                // aggregates

                ws.Cells["A2"].Value = "Start date";
                ws.Cells["B2"].Value = startDate;
                ws.Row(2).Style.Font.Size = 12;
                ws.Row(2).Height = 15;

                ws.Cells["A3"].Value = "End date";
                ws.Cells["B3"].Value = endDate;
                ws.Row(3).Style.Font.Size = 12;
                ws.Row(3).Height = 15;

                ws.Cells["A4"].Value = "Initial Capital";
                ws.Cells["B4"].Value = Math.Round(strategy.Budget) + " $";
                ws.Row(4).Style.Font.Size = 12;
                ws.Row(4).Height = 15;

                ws.Cells["A5"].Value = "Ending Capital";
                ws.Cells["B5"].Value = Math.Round(endingCapital) + " $";
                ws.Row(5).Style.Font.Size = 12;
                ws.Row(5).Height = 15;

                ws.Cells["A6"].Value = "Total Net Profit";
                ws.Cells["B6"].Value = Math.Round(nettProfit.Value) + " $";
                ws.Row(6).Style.Numberformat.Format = "@";
                ws.Row(6).Style.Font.Size = 12;
                ws.Row(6).Height = 15;

                ws.Cells["A7"].Value = "Total N. of Trades";
                ws.Cells["B7"].Value = numberOfTrades;
                ws.Row(7).Style.Font.Size = 12;
                ws.Row(7).Height = 15;

                ws.Cells["A8"].Value = "Percentage Winners";
                ws.Cells["B8"].Value = Math.Round(winnersPercentage) + " %";
                ws.Row(8).Style.Font.Size = 12;
                ws.Row(8).Height = 15;

                ws.Cells["A9"].Value = "Profit Factor";
                ws.Cells["B9"].Value = Math.Abs(Math.Round(profitFactor.Value, 2));
                ws.Row(9).Style.Font.Size = 12;
                ws.Row(9).Height = 15;

                ws.Cells["A10"].Value = "Max. Drawdown $";
                ws.Cells["B10"].Value = Math.Round(_drawdown.GetMaxDrawdown()) + " $";
                ws.Row(10).Style.Font.Size = 12;
                ws.Row(10).Height = 15;

                ws.Cells["A11"].Value = "Max. Drawdown %";
                ws.Cells["B11"].Value = Math.Round(_drawdown.GetMaxDrawdownPercentage()) + " %";
                ws.Row(11).Style.Font.Size = 12;
                ws.Row(11).Height = 15;
                var ieb11 = ws.IgnoredErrors.Add(ws.Cells["B11"]);
                var ieb8 = ws.IgnoredErrors.Add(ws.Cells["B8"]);
                ieb11.NumberStoredAsText = true;
                ieb8.NumberStoredAsText = true;

                // Data
                var orders = strategy.OrderedGeneratedOrdersHistory.Select(x => new
                {
                    x.Symbol,
                    x.OpenPrice,
                    x.OpenDate,
                    x.OpenIndicators,
                    x.ClosePrice,
                    x.CloseDate,
                    x.CloseIndicators,
                    x.Position,
                    x.AdditionallyBoughtPositions,
                    x.BudgetInvestedPercentage,
                    x.ProfitLoss,
                    x.CumulatedCapital,
                }).Where(x => x.CloseDate != null);

                var range = ws.Cells["A25"].LoadFromCollection(orders, true);
                range.AutoFitColumns();

                // format datetime
                ws.Column(3).Style.Numberformat.Format = "dd-mm-yyyy";
                ws.Column(6).Style.Numberformat.Format = "dd-mm-yyyy";

                // formats the header
                ws.Cells["A24"].Value = "Data";
                ws.Cells["A24:G24"].Merge = true;
                ws.Row(24).Style.Font.Size = 18;

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
    }
}