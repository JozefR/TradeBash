using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using TradeBash.Core.Entities.Strategy;

namespace TradeBash.Infrastructure.Services
{
    public class ExcelReporting : IExcelReporting
    {
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
                var strategyName = $"{strategy.Name} Report";
                var ws = package.Workbook.Worksheets.Add(strategyName);

                var nettProfit = strategy.GeneratedOrders.Sum(x => x.ProfitLoss);

                var profitTrades = strategy.GeneratedOrders.Where(x => x.ProfitLoss > 0).Sum(x => x.ProfitLoss);
                var lossTrades = strategy.GeneratedOrders.Where(x => x.ProfitLoss < 0).Sum(x => x.ProfitLoss);
                var profitFactor = profitTrades / lossTrades;

                var endingCapital = strategy.GeneratedOrders.OrderBy(x => x.CloseDate).Last().CumulatedCapital;

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
                    drawDown.Calculate(order.CumulatedCapital);
                }

                // Results
                // header
                ws.Cells["A1"].Value = strategyName;
                ws.Cells["A1:G1"].Merge = true;

                ws.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Row(1).Style.Font.Size = 18;
                // aggregates
                ws.Cells["A2"].Value = "Initial Capital";
                ws.Cells["B2"].Value = strategy.Budget;
                ws.Row(2).Style.Font.Bold = true;

                ws.Cells["A3"].Value = "Ending Capital";
                ws.Cells["B3"].Value = endingCapital;
                ws.Row(3).Style.Font.Bold = true;

                ws.Cells["A4"].Value = "Total Net Profit";
                ws.Cells["B4"].Value = nettProfit;
                ws.Row(4).Style.Font.Bold = true;

                ws.Cells["A5"].Value = "Total N. of Trades";
                ws.Cells["B5"].Value = numberOfTrades;
                ws.Row(5).Style.Font.Bold = true;

                ws.Cells["A6"].Value = "Total Tested History";
                ws.Cells["B6"].Value = testedHistory;
                ws.Row(6).Style.Font.Bold = true;

                ws.Cells["A7"].Value = "Percentage Winners";
                ws.Cells["B7"].Value = Math.Round(winnersPercentage, 2);
                ws.Row(7).Style.Font.Bold = true;

                ws.Cells["A8"].Value = "Profit Factor";
                ws.Cells["B8"].Value = Math.Round(profitFactor.Value, 2);
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
                    x.Position,
                    x.BudgetInvestedPercentage,
                    x.ProfitLoss,
                    x.CumulatedCapital
                }).OrderBy(x => x.OpenDate);

                var range = ws.Cells["A11"].LoadFromCollection(orders, true);
                ws.Cells[1, 4, strategy.GeneratedOrders.Count + 2, 5].Style.Numberformat.Format = "dd-mm-yyyy";
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
    }
}