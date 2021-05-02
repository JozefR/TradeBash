#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using TradeBash.SharedKernel;

namespace TradeBash.Core.Entities.Strategy
{
    public class StrategyStock : BaseEntity
    {
        public string Symbol { get; private set; }

        public string Label { get; private set; }

        private readonly List<CalculatedStock> _calculatedStocksHistory;
        public IReadOnlyCollection<CalculatedStock> CalculatedStocksHistory =>
            _calculatedStocksHistory
            .OrderBy(x => x.Date)
            .ToList();

        private int? _simpleMovingAverageParameter;
        private int? _relativeStrengthIndexParameter;

        private StrategyStock()
        {
            _calculatedStocksHistory = new List<CalculatedStock>();
        }

        public static StrategyStock From(
            string symbol,
            string label,
            int? simpleMovingAverageParameter,
            int? relativeStrengthIndexParameter)
        {
            var strategyStock = new StrategyStock
            {
                Symbol = symbol,
                Label = label,
                _simpleMovingAverageParameter = simpleMovingAverageParameter,
                _relativeStrengthIndexParameter = relativeStrengthIndexParameter
            };

            return strategyStock;
        }

        public void CalculateForStock(
            string symbol,
            DateTime date,
            double open,
            double close)
        {
            var sma = CalculateSimpleMovingAverage();
            var rsi = CalculateRelativeStrengthIndex();

            var stock = CalculatedStock.From(symbol, date, open, close, sma, rsi);

            _calculatedStocksHistory.Add(stock);
        }

        private double? CalculateSimpleMovingAverage()
        {
            if (!_simpleMovingAverageParameter.HasValue) return null;

            if (_calculatedStocksHistory.Count < _simpleMovingAverageParameter) return null;

            var prices = _calculatedStocksHistory.TakeLast(_simpleMovingAverageParameter.Value).Select(x => x.Close);
            double? average = prices.AsQueryable().Average();

            return average;
        }

        private double? CalculateRelativeStrengthIndex()
        {
            if (!_relativeStrengthIndexParameter.HasValue) return null;

            if (_calculatedStocksHistory.Count <= _relativeStrengthIndexParameter + 15) return null;

            var price = _calculatedStocksHistory.Select(x => x.Close).ToArray();
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

            double avrg = gain / _relativeStrengthIndexParameter.Value;
            double avrl = loss / _relativeStrengthIndexParameter.Value;
            double rs = gain / loss;

            if (double.IsNaN(rs))
            {
                rs = 0;
            }

            rsi[_relativeStrengthIndexParameter.Value] = 100 - (100 / (1 + rs));

            for (int i = _relativeStrengthIndexParameter.Value + 1; i < price.Length; ++i)
            {
                var diff = price[i] - price[i - 1];

                if (diff >= 0)
                {
                    avrg = ((avrg * (_relativeStrengthIndexParameter.Value - 1)) + diff) /
                           _relativeStrengthIndexParameter.Value;
                    avrl = (avrl * (_relativeStrengthIndexParameter.Value - 1)) / _relativeStrengthIndexParameter.Value;
                }
                else
                {
                    avrl = ((avrl * (_relativeStrengthIndexParameter.Value - 1)) - diff) /
                           _relativeStrengthIndexParameter.Value;
                    avrg = (avrg * (_relativeStrengthIndexParameter.Value - 1)) / _relativeStrengthIndexParameter.Value;
                }

                rs = avrg / avrl;

                if (double.IsNaN(rs))
                {
                    rs = 0;
                }

                rsi[i] = 100 - (100 / (1 + rs));
            }

            return rsi[_calculatedStocksHistory.Count - 1];
        }
    }
}