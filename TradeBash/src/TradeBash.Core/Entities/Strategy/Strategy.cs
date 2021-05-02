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

        private int? _smaShortParameter;

        private int? _smaLongParameter;

        private int? _rsaParameter;

        public IReadOnlyCollection<StrategyStock> StocksHistory => _stocksHistory;
        private readonly List<StrategyStock> _stocksHistory;

        public IReadOnlyCollection<GeneratedOrder> GeneratedOrders => _generatedOrders;
        private readonly List<GeneratedOrder> _generatedOrders;

        private Strategy()
        {
            Name = string.Empty;
            Budget = Double.MinValue;
            _stocksHistory = new List<StrategyStock>();
            _generatedOrders = new List<GeneratedOrder>();
        }

        public static Strategy From(
            string name,
            double budget,
            int? smaShort = null,
            int? smaLong = null,
            int? rsiParameter = null)
        {
            var strategy = new Strategy
            {
                Name = name,
                Budget = budget,
                _smaShortParameter = smaShort,
                _smaLongParameter = smaLong,
                _rsaParameter = rsiParameter,
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
                _rsaParameter);

            foreach (var stockHistory in stock.OrderedHistory)
            {
                strategyStock.CalculateForStock(
                    stock.Symbol, 
                    stockHistory.Date, 
                    stockHistory.Open, 
                    stockHistory.Close);
            }

            _stocksHistory.Add(strategyStock);
        }

        public void RunBackTestForDate()
        {
            // buy if rsi < 2;
            // sell if sma > 10
            var index = 0;
            CalculatedStock? generatedSignal = null;
            foreach (var inDate in GetHistoryInDates())
            {
                foreach (var strategyStock in _stocksHistory)
                {
                    if (index >= strategyStock.OrderedStocksHistory.Count) continue;

                    var currentStock = strategyStock.OrderedStocksHistory.ToList()[index];

                    if (currentStock.RSI == 0) continue;
                    if (currentStock.Date != inDate) continue;

                    if (currentStock.RSI < _rsaParameter ||
                        (generatedSignal != null && currentStock.RSI < generatedSignal.RSI))
                    {
                        generatedSignal = currentStock;
                    }

                    var openPosition = GeneratedOrders.FirstOrDefault(x => x.CloseDate == null && x.Symbol == strategyStock.Symbol);

                    if (openPosition == null) continue;

                    if (currentStock.SMAShort > currentStock.Close)
                    {
                        openPosition.ClosePosition(currentStock.Close, currentStock.Date);
                        openPosition.CalculateProfitLoss();
                    }
                }

                if (generatedSignal != null)
                {
                    var openPositions = NumberOfCurrentOpenedPositions(generatedSignal);
                    var generatedOrder = GeneratedOrder.OpenPosition(
                        generatedSignal.Symbol,
                        generatedSignal.Open,
                        generatedSignal.Date);
                    generatedOrder.CalculateNumberOfStockForPosition(Budget, openPositions);

                    _generatedOrders.Add(generatedOrder);
                }

                index++;
            }
        }

        public void RemovePreviousIndicatorCalculations()
        {
            _generatedOrders.Clear();
        }

        public void RemovePreviousGeneratedOrders()
        {
            _generatedOrders.Clear();
        }

        public List<DateTime> GetHistoryInDates()
        {
            return _stocksHistory.FirstOrDefault()!.OrderedStocksHistory.Select(x => x.Date).ToList();
        }

        private int NumberOfCurrentOpenedPositions(CalculatedStock generatedSignal)
        {
            return GeneratedOrders.Count(x => x.CloseDate == null && x.Symbol == generatedSignal.Symbol);
        }
    }
}