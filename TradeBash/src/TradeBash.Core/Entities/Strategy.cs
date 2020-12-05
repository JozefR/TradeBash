using System;
using System.Collections.Generic;
using TradeBash.SharedKernel;

namespace TradeBash.Core.Entities
{
    public class Strategy : BaseEntity, IAggregateRoot
    {
        public double SimpleMovingAverage { get; set; } // Value object

        public double RelativeStrengthIndex { get; set; } // Value object

        public IList<Stock> StocksHistory { get; set; }

        public Strategy()
        {
            StocksHistory = new List<Stock>();
        }

        public static Strategy CalculateForStock(DateTime date, string symbol, double open, double close, string label)
        {
            var strategy = new Strategy();

            var stock = Stock.From(date, symbol, open, close, label);
            strategy.StocksHistory.Add(stock);

            strategy.CalculateSimpleMovingAverage();
            strategy.CalculateRelativeStrengthIndex();
            
            return strategy;
        }

        private void CalculateSimpleMovingAverage()
        {
            if (StocksHistory.Count > 5)
            {
                // calculate
            }

            throw new NotImplementedException();
        }

        private void CalculateRelativeStrengthIndex()
        {
            if (StocksHistory.Count > 12)
            {
                // calculate
            }

            throw new NotImplementedException();
        }
    }
}