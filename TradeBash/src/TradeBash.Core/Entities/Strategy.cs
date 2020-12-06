using System;
using System.Collections.Generic;
using System.Linq;
using TradeBash.SharedKernel;

namespace TradeBash.Core.Entities
{
    public class Strategy : BaseEntity, IAggregateRoot
    {
        public int SimpleMovingAverageParameter { get; set; }

        public int RelativeStrengthIndexParameter { get; set; }

        public IList<Stock> StocksHistory { get; set; }

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
            decimal open,
            decimal close,
            string label)
        {
            decimal? sma = CalculateSimpleMovingAverage();
            decimal? rsi = CalculateRelativeStrengthIndex();

            var stock = Stock.From(date, symbol, open, close, label, sma, rsi);
            StocksHistory.Add(stock);

            return stock;
        }

        private decimal? CalculateSimpleMovingAverage()
        {
            if (StocksHistory.Count <= 5) return null;

            var prices = StocksHistory.TakeLast(SimpleMovingAverageParameter).Select(x => x.Close);
            decimal? average = Queryable.Average(prices.AsQueryable());

            return average;
        }

        private decimal? CalculateRelativeStrengthIndex()
        {
            if (StocksHistory.Count > 12)
            {
                // calculate
            }

            return null;
        }
    }
}