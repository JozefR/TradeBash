#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using TradeBash.Core.Entities.Warehouse;
using TradeBash.SharedKernel;
using TradeBash.SharedKernel.Interfaces;

namespace TradeBash.Core.Entities.Strategy
{
    public class Strategy : BaseEntity, IAggregateRoot
    {
        public string Name { get; private set; }

        public double Budget { get; private set; }

        private double cumulatedBudget;

        private int? _smaShortParameter;
        private int? _smaLongParameter;
        private int? _rsiParameter;

        public ICollection<StrategyStock> StrategyStocksHistory { get; set; }
        public ICollection<GeneratedOrder> GeneratedOrders { get; set; }

        public IReadOnlyCollection<GeneratedOrder> OrderedGeneratedOrdersHistory => GeneratedOrders.OrderBy(x => x.CloseDate).ToList();

        private Strategy()
        {
            Name = string.Empty;
            Budget = double.MinValue;
            StrategyStocksHistory = new List<StrategyStock>();
            GeneratedOrders = new List<GeneratedOrder>();
        }

        public static Strategy From(
            string name,
            double budget,
            int smaShort,
            int rsiParameter)
        {
            var strategy = new Strategy
            {
                Name = name,
                Budget = budget,
                _smaShortParameter = smaShort,
                _rsiParameter = rsiParameter,
            };

            return strategy;
        }

        public static Strategy From(
            string name,
            double budget,
            int smaShort,
            int smaLong,
            int rsiParameter)
        {
            var strategy = new Strategy
            {
                Name = name,
                Budget = budget,
                _smaShortParameter = smaShort,
                _smaLongParameter = smaLong,
                _rsiParameter = rsiParameter,
            };

            return strategy;
        }

        public static Strategy From(
            string name,
            int budget,
            int smaShort,
            int smaLong,
            int rsiParameter)
        {
            var strategy = new Strategy
            {
                Name = name,
                Budget = budget,
                _smaShortParameter = smaShort,
                _smaLongParameter = smaLong,
                _rsiParameter = rsiParameter,
            };

            return strategy;
        }

        public void RunCalculationFor(Stock stock)
        {
            var strategyStock = StrategyStock.From(
                stock.Symbol,
                stock.Name,
                _smaShortParameter,
                _smaLongParameter,
                _rsiParameter);

            foreach (var stockHistory in stock.OrderedHistory)
            {
                strategyStock.CalculateForStock(
                    stock.Symbol, 
                    stockHistory.Date, 
                    stockHistory.Open, 
                    stockHistory.Close);
            }

            StrategyStocksHistory.Add(strategyStock);
        }

        public void RunBackTestForDate()
        {
            // buy if rsi < 2;
            // sell if sma > 10
            CalculatedStock? generatedSignal = null;
            cumulatedBudget = Budget;
            foreach (var inDate in GetHistoryInDates())
            {
                foreach (var strategyStock in StrategyStocksHistory)
                {
                    var currentStock = strategyStock.CalculatedOrderedStocksHistory.FirstOrDefault(x => x.Date == inDate);

                    if (currentStock == null) continue;
                    if (StrategyGuard.RsiNotCalculated(currentStock)) continue;

                    var openPositions = GetCurrentNotClosedPositionsFor(currentStock);

                    foreach (var openPosition in openPositions)
                    {
                        ClosePisitionForSma(openPosition, currentStock);
                    }

                    if (GenerateBuySignalForRsiIfCurrentStockLower(generatedSignal, currentStock, 10))
                    {
                        generatedSignal = currentStock;
                    }
                }

                if (generatedSignal != null)
                {
                    OpenPositionAndGenerateOrder(generatedSignal);

                    generatedSignal = null;
                }

                Console.WriteLine($"calculating for date: {inDate}");
            }
        }

        private void ClosePisitionForSma(GeneratedOrder openPosition, CalculatedStock currentStock)
        {
            if (currentStock.SMAShort < currentStock.Close)
            {
                var profitLoss = openPosition.ClosePosition(currentStock.Close, currentStock.Date);
                cumulatedBudget += profitLoss;
                openPosition.SetCumulatedCapital(cumulatedBudget);
            }
        }

        private void OpenPositionAndGenerateOrder(CalculatedStock generatedSignal)
        {
            var generatedOrder = GeneratedOrder.OpenPosition(
                generatedSignal.Symbol,
                generatedSignal.Open,
                generatedSignal.Date);
            generatedOrder.PercentageForStocksFixedMM(Budget, 5);
            GeneratedOrders.Add(generatedOrder);
        }

        private bool GenerateBuySignalForRsiIfCurrentStockLower(CalculatedStock? generatedSignal, CalculatedStock currentStock, int rsiThreshold)
        {
            if (currentStock.RSI < rsiThreshold || (generatedSignal != null && currentStock.RSI < generatedSignal.RSI))
            {
                return true;
            }

            return false;
        }

        private IEnumerable<GeneratedOrder> GetCurrentNotClosedPositionsFor(CalculatedStock currentStock)
        {
            return GeneratedOrders.Where(x => x.CloseDate == null && x.Symbol == currentStock.Symbol);
        }

        private List<DateTime> GetHistoryInDates()
        {
            return StrategyStocksHistory.FirstOrDefault()!.CalculatedOrderedStocksHistory.Select(x => x.Date).ToList();
        }

        private int NumberOfCurrentOpenedPositions(CalculatedStock generatedSignal)
        {
            return GeneratedOrders.Count(x => x.CloseDate == null && x.Symbol == generatedSignal.Symbol);
        }
    }
}