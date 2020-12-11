using System;
using System.Collections.Generic;
using System.Linq;
using TradeBash.SharedKernel;
using TradeBash.SharedKernel.Interfaces;

namespace TradeBash.Core.Entities
{
    public class Strategy : BaseEntity, IAggregateRoot
    {
        private int _simpleMovingAverageParameter;

        private int _relativeStrengthIndexParameter;

        private readonly List<Stock> _stocksHistory;

        public IReadOnlyCollection<Stock> StocksHistory => _stocksHistory;

        private Strategy()
        {
            _stocksHistory = new List<Stock>();
        }

        public static Strategy SetIndicatorParameters(
            int smaParameter,
            int rsiParameter)
        {
            var strategy = new Strategy
            {
                _simpleMovingAverageParameter = smaParameter,
                _relativeStrengthIndexParameter = rsiParameter,
            };

            return strategy;
        }

        public Stock AddStock(
            DateTime date,
            string symbol,
            double open,
            double close,
            string label)
        {
            var sma = CalculateSimpleMovingAverage();
            var rsi = CalculateRelativeStrengthIndex();

            var stock = Stock.From(date, symbol, open, close, label, sma, rsi);
            _stocksHistory.Add(stock);

            return stock;
        }

        private double? CalculateSimpleMovingAverage()
        {
            if (_stocksHistory.Count < _simpleMovingAverageParameter) return null;

            var prices = _stocksHistory.TakeLast(_simpleMovingAverageParameter).Select(x => x.Close);
            double? average = prices.AsQueryable().Average();

            return average;
        }

        private double? CalculateRelativeStrengthIndex()
        {
            if (_stocksHistory.Count <= _relativeStrengthIndexParameter) return null;

            var price = _stocksHistory.Select(x => x.Close).ToArray();
            var rsi = new double[price.Length];

            double gain = 0.0;
            double loss = 0.0;

            rsi[0] = 0.0;
            for (int i = 1; i <= _relativeStrengthIndexParameter; ++i)
            {
                var diff = price[i] - price[i - 1];
                if (diff >= 0)
                {
                    gain += diff;
                }
                else
                {
                    loss -= diff;
                }
            }

            double avrg = gain / _relativeStrengthIndexParameter;
            double avrl = loss / _relativeStrengthIndexParameter;
            double rs = gain / loss;
            rsi[_relativeStrengthIndexParameter] = 100 - (100 / (1 + rs));

            for (int i = _relativeStrengthIndexParameter + 1; i < price.Length; ++i)
            {
                var diff = price[i] - price[i - 1];

                if (diff >= 0)
                {
                    avrg = ((avrg * (_relativeStrengthIndexParameter - 1)) + diff) / _relativeStrengthIndexParameter;
                    avrl = (avrl * (_relativeStrengthIndexParameter - 1)) / _relativeStrengthIndexParameter;
                }
                else
                {
                    avrl = ((avrl * (_relativeStrengthIndexParameter - 1)) - diff) / _relativeStrengthIndexParameter;
                    avrg = (avrg * (_relativeStrengthIndexParameter - 1)) / _relativeStrengthIndexParameter;
                }

                rs = avrg / avrl;

                rsi[i] = 100 - (100 / (1 + rs));
            }

            return rsi[_stocksHistory.Count - 1];
        }
    }
}