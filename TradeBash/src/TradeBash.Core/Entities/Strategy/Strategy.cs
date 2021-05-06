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

        private int? Budget { get; set; }

        private int? _smaShortParameter;

        private int? _smaLongParameter;

        private int? _rsiParameter;

        public ICollection<StrategyStock> StrategyStocksHistory { get; set; }

        public ICollection<GeneratedOrder> GeneratedOrders { get; set; }

        private Strategy()
        {
            Name = string.Empty;
            Budget = int.MinValue;
            StrategyStocksHistory = new List<StrategyStock>();
            GeneratedOrders = new List<GeneratedOrder>();
        }

        public static Strategy From(
            string name,
            int smaShort,
            int rsiParameter)
        {
            var strategy = new Strategy
            {
                Name = name,
                _smaShortParameter = smaShort,
                _rsiParameter = rsiParameter,
            };

            return strategy;
        }

        public static Strategy From(
            string name,
            int smaShort,
            int smaLong,
            int rsiParameter)
        {
            var strategy = new Strategy
            {
                Name = name,
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
                _smaShortParameter = smaShort,
                _smaLongParameter = smaLong,
                _rsiParameter = rsiParameter,
            };

            return strategy;
        }

        public void RunCalculationForStock(Stock stock)
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
            // fixed money management
            var index = 0;
            CalculatedStock? generatedSignal = null;
            foreach (var inDate in GetHistoryInDates())
            {
                foreach (var strategyStock in StrategyStocksHistory)
                {
                    if (StrategyGuard.IndexOutOfRange(index, strategyStock)) continue;

                    var currentStock = strategyStock.CalculatedStocksHistory.ToList()[index];

                    if (StrategyGuard.RsiNotCalculated(currentStock)) continue;
                    if (StrategyGuard.NotSameDate(currentStock, inDate)) continue;

                    if (GenerateBuySignalForRsiIfCurrentStockLower(generatedSignal, currentStock))
                    {
                        generatedSignal = currentStock;
                    }

                    var openPosition = GetCurrentNotClosedPositionFor(currentStock);
                    if (openPosition == null) continue;

                    if (currentStock.SMAShort > currentStock.Close)
                    {
                        openPosition.ClosePosition(currentStock.Close, currentStock.Date);
                        openPosition.CalculateProfitLoss();
                    }
                }

                if (generatedSignal != null)
                {
                    var generatedOrder = GeneratedOrder.OpenPosition(generatedSignal.Symbol, generatedSignal.Open, generatedSignal.Date);
                    generatedOrder.NumberOfStocksForPosition(100);

                    GeneratedOrders.Add(generatedOrder);
                }

                index++;
            }
        }

        private bool GenerateBuySignalForRsiIfCurrentStockLower(CalculatedStock? generatedSignal, CalculatedStock currentStock)
        {
            if (currentStock.RSI < _rsiParameter || (generatedSignal != null && currentStock.RSI < generatedSignal.RSI))
            {
                return true;
            }

            return false;
        }

        private GeneratedOrder? GetCurrentNotClosedPositionFor(CalculatedStock currentStock)
        {
            return GeneratedOrders.FirstOrDefault(x => x.CloseDate == null && x.Symbol == currentStock.Symbol);
        }

        private List<DateTime> GetHistoryInDates()
        {
            return StrategyStocksHistory.FirstOrDefault()!.CalculatedStocksHistory.Select(x => x.Date).ToList();
        }

        private int NumberOfCurrentOpenedPositions(CalculatedStock generatedSignal)
        {
            return GeneratedOrders.Count(x => x.CloseDate == null && x.Symbol == generatedSignal.Symbol);
        }
    }
}