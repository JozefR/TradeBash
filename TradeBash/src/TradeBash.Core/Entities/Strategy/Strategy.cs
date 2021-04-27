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

        private int? _simpleMovingAverageParameter;

        private int? _relativeStrengthIndexParameter;

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
            int? smaParameter = null,
            int? rsiParameter = null)
        {
            var strategy = new Strategy
            {
                Name = name,
                Budget = budget,
                _simpleMovingAverageParameter = smaParameter,
                _relativeStrengthIndexParameter = rsiParameter,
            };

            return strategy;
        }

        public void RunCalculation(IEnumerable<Stock> stocks)
        {
            foreach (var stock in stocks)
            {
                var orderedHistory = stock.History.OrderBy(x => x.Date);
                var strategyStock = StrategyStock.From(
                    stock.Symbol,
                    stock.Name,
                    _simpleMovingAverageParameter,
                    _relativeStrengthIndexParameter);

                foreach (var stockHistory in orderedHistory)
                {
                    strategyStock.CalculateForStock(stock.Symbol, stockHistory.Date, stockHistory.Open, stockHistory.Close);
                    _stocksHistory.Add(strategyStock);
                }
            }
        }

        public void RunBackTest()
        {
            if (!_stocksHistory.Any())
            {
                throw new Exception();
            }

            // buy if rsi < 2;
            // sell if sma > 10
            CalculatedStock? generatedSignal = null;
            var dates = _stocksHistory.FirstOrDefault()!.CalculatedStocksHistory.Select(x => x.Date).ToList();
            var index = 0;
            foreach (var date in dates)
            {
                foreach (var strategyStock in _stocksHistory)
                {
                    if (index >= strategyStock.CalculatedStocksHistory.Count) continue;

                    var currentStock = strategyStock.CalculatedStocksHistory.ToList()[index];

                    if (currentStock.RSI == 0) continue;
                    if (currentStock.Date != date) continue;

                    if (currentStock.RSI < _relativeStrengthIndexParameter || (generatedSignal != null && currentStock.RSI < generatedSignal.RSI))
                    {
                        generatedSignal = currentStock;
                    }

                    var openPosition = GeneratedOrders.FirstOrDefault(x => x.CloseDate == null && x.Symbol == strategyStock.Symbol);

                    if (openPosition == null) continue;

                    if (currentStock.SMA > currentStock.Close)
                    {
                        openPosition.ClosePosition(currentStock.Close, currentStock.Date);
                        openPosition.CalculateProfitLoss();
                    }
                }

                if (generatedSignal != null)
                {
                    var openPositions = NumberOfCurrentOpenedPositions(generatedSignal);
                    var generatedOrder = GeneratedOrder.OpenPosition(generatedSignal.Symbol, generatedSignal.Open, generatedSignal.Date);
                    generatedOrder.CalculateNumberOfStockForPosition(Budget, openPositions);

                    _generatedOrders.Add(generatedOrder);

                    generatedSignal = null;
                }

                index++;
            }
        }

        private int NumberOfCurrentOpenedPositions(CalculatedStock generatedSignal)
        {
            return GeneratedOrders.Count(x => x.CloseDate == null && x.Symbol == generatedSignal.Symbol);
        }
    }
}