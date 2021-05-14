#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using TradeBash.Core.Entities.Warehouse;
using TradeBash.Core.Services;
using TradeBash.SharedKernel;
using TradeBash.SharedKernel.Interfaces;

namespace TradeBash.Core.Entities.Strategy
{
    public class Strategy : BaseEntity, IAggregateRoot
    {
        public string Name { get; private set; }

        public double Budget { get; private set; }

        private Drawdown _drawdown;

        private double cumulatedBudget;

        private double cumulatedBudgetFromLowPride;

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
            _drawdown = new Drawdown();
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
                    stockHistory.Close,
                    stockHistory.Low);
            }

            StrategyStocksHistory.Add(strategyStock);
        }

        public void RunShortSmaRsi()
        {
            // buy if rsi < 2;
            // sell if sma > 10
            CalculatedStock? generatedSignal = null;
            cumulatedBudget = Budget;
            cumulatedBudgetFromLowPride = Budget;
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
                        ClosePositionForSma(openPosition, currentStock);
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

                Console.WriteLine($"Generate orders for date: {inDate}");
            }
        }

        public void RunShortSmaLongSmaRsi()
        {
            // buy if rsi < 2;
            // sell if sma > 10
            CalculatedStock? generatedSignal = null;
            cumulatedBudget = Budget;
            cumulatedBudgetFromLowPride = Budget;
            foreach (var inDate in GetHistoryInDates())
            {
                foreach (var strategyStock in StrategyStocksHistory)
                {
                    var currentStock = strategyStock.CalculatedOrderedStocksHistory.FirstOrDefault(x => x.Date == inDate);

                    if (currentStock == null) continue;
                    if (StrategyGuard.LongSmaIsGreaterThenPrice(currentStock)) continue;
                    if (StrategyGuard.RsiNotCalculated(currentStock)) continue;

                    var openPositions = GetCurrentNotClosedPositionsFor(currentStock);

                    foreach (var openPosition in openPositions)
                    {
                        CalculateMaxDrawdownFromLowPrice(openPosition, currentStock);
                        ClosePositionForSma(openPosition, currentStock);
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

                Console.WriteLine($"Generate orders for date: {inDate}");
            }
        }

        private void CalculateMaxDrawdownFromLowPrice(GeneratedOrder openPosition, CalculatedStock currentStock)
        {
            // todo: calculate max drawdown from low
        }

        private void ClosePositionForSma(GeneratedOrder openPosition, CalculatedStock currentStock)
        {
            if (currentStock.SMAShort < currentStock.Close)
            {
                var profitLoss = openPosition.ClosePosition(currentStock.Close, currentStock.Date);
                cumulatedBudget += profitLoss;
                _drawdown.Calculate(cumulatedBudget);
                openPosition.SetCumulatedCapital(cumulatedBudget);
                openPosition.SetMaxDrawdown(_drawdown.TmpDrawDown, Budget);
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