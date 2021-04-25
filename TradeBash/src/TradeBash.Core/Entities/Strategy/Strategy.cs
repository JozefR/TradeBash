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

        private readonly List<StrategyStock> _stocksHistory;

        public IReadOnlyCollection<StrategyStock> StocksHistory => _stocksHistory;

        private Strategy()
        {
            _stocksHistory = new List<StrategyStock>();
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

        /*public void RunBackTest()
        {
            if (!_stocksHistory.Any())
            {
                throw new Exception();
            }

            foreach (var stock in _stocksHistory)
            {
                //  strategy logic
                stock.SetStrategySignal("Buy");
                stock.FromBackTest(200);
            }

            throw new NotImplementedException();
        }*/
    }
}