using System;
using System.Collections.Generic;
using System.Linq;
using TradeBash.SharedKernel;
using TradeBash.SharedKernel.Interfaces;

namespace TradeBash.Core.Entities
{
    public class Strategy : BaseEntity, IAggregateRoot
    {
        public int SimpleMovingAverageParameter { get; set; }

        public int RelativeStrengthIndexParameter { get; set; }

        public IList<Stock> StocksHistory { get; }

        private Strategy()
        {
            StocksHistory = new List<Stock>();
        }

        public static Strategy CalculateForStock(
            int smaParameter,
            int rsiParameter)
        {
            var strategy = new Strategy
            {
                SimpleMovingAverageParameter = smaParameter,
                RelativeStrengthIndexParameter = rsiParameter,
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
            StocksHistory.Add(stock);

            return stock;
        }

        private double? CalculateSimpleMovingAverage()
        {
            if (StocksHistory.Count < SimpleMovingAverageParameter) return null;

            var prices = StocksHistory.TakeLast(SimpleMovingAverageParameter).Select(x => x.Close);
            double? average = prices.AsQueryable().Average();

            return average;
        }

        private double? CalculateRelativeStrengthIndex()
        {
            if (StocksHistory.Count <= RelativeStrengthIndexParameter) return null;

            var price = StocksHistory.Select(x => x.Close).ToArray();
            var rsi = new double[price.Length];

            double gain = 0.0;
            double loss = 0.0;

            rsi[0] = 0.0;
            for (int i = 1; i <= RelativeStrengthIndexParameter; ++i)
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

            double avrg = gain / RelativeStrengthIndexParameter;
            double avrl = loss / RelativeStrengthIndexParameter;
            double rs = gain / loss;
            rsi[RelativeStrengthIndexParameter] = 100 - (100 / (1 + rs));

            for (int i = RelativeStrengthIndexParameter + 1; i < price.Length; ++i)
            {
                var diff = price[i] - price[i - 1];

                if (diff >= 0)
                {
                    avrg = ((avrg * (RelativeStrengthIndexParameter - 1)) + diff) / RelativeStrengthIndexParameter;
                    avrl = (avrl * (RelativeStrengthIndexParameter - 1)) / RelativeStrengthIndexParameter;
                }
                else
                {
                    avrl = ((avrl * (RelativeStrengthIndexParameter - 1)) - diff) / RelativeStrengthIndexParameter;
                    avrg = (avrg * (RelativeStrengthIndexParameter - 1)) / RelativeStrengthIndexParameter;
                }

                rs = avrg / avrl;

                rsi[i] = 100 - (100 / (1 + rs));
            }

            return rsi[StocksHistory.Count - 1];
        }
    }
}