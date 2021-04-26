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
        private string _name;

        private int? _simpleMovingAverageParameter;

        private int? _relativeStrengthIndexParameter;

        public IReadOnlyCollection<StrategyStock> StocksHistory => _stocksHistory;
        private readonly List<StrategyStock> _stocksHistory;

        public IReadOnlyCollection<GeneratedOrder> GeneratedOrders => _generatedOrders;
        private readonly List<GeneratedOrder> _generatedOrders;

        private Strategy()
        {
            _stocksHistory = new List<StrategyStock>();
            _generatedOrders = new List<GeneratedOrder>();
        }

        public static Strategy From(
            string name,
            int? smaParameter = null,
            int? rsiParameter = null)
        {
            var strategy = new Strategy
            {
                _name = name,
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
                    strategyStock.CalculateForStock(stockHistory.Date, stockHistory.Open, stockHistory.Close);
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

                    var openPosition1 = GeneratedOrders.FirstOrDefault();

                    var openPosition = strategyStock.CalculatedStocksHistory
                        .LastOrDefault(x => x.StrategySignal == "Buy" || x.StrategySignal == "Sell");

                    if (openPosition == null) continue;

                    if (openPosition.StrategySignal == "Buy" && currentStock.SMA > currentStock.Close)
                    {
                        currentStock.SellStock();
                    }
                }

                if (generatedSignal != null)
                {
                    _generatedOrders.Add(GeneratedOrder.OpenPosition(generatedSignal.Open, generatedSignal.Date, 100));
                }

                index++;
            }
        }
    }
}