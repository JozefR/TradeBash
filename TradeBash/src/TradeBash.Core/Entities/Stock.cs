using System;
using TradeBash.SharedKernel;

namespace TradeBash.Core.Entities
{
    public class Stock : BaseEntity
    {
        public DateTime Date { get; private set; }
        
        public string Symbol { get; private set; }
        
        public double Open { get; private set; }
        
        public double Close { get; private set; }
        
        public string Label { get; private set; }

        public double? SMA { get; private set; }

        public double? RSI { get; private set; }

        public double? ProfitLoss { get; private set; }

        public int StrategyId { get; set; }

        private Stock() { }

        public static Stock From(
            DateTime date,
            string symbol,
            double open,
            double close,
            string label,
            double? sma,
            double? rsi)
        {
            var entity = new Stock
            {
                Date = date,
                Symbol = symbol,
                Open = open,
                Close = close,
                Label = label,
                SMA = sma,
                RSI = rsi
            };

            return entity;
        }

        public Stock FromBackTest(double? profitLoss)
        {
            ProfitLoss = profitLoss;

            return this;
        }
    }
}