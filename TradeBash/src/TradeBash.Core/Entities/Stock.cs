using System;
using TradeBash.SharedKernel;

namespace TradeBash.Core.Entities
{
    public class Stock : BaseEntity
    {
        public DateTime Date { get; set; }
        
        public string Symbol { get; set; }
        
        public decimal Open { get; set; }
        
        public decimal Close { get; set; }
        
        public string Label { get; set; }

        public decimal? SMA { get; set; }

        public decimal? RSI { get; set; }

        public int StrategyId { get; set; }
        public Strategy Strategy { get; set; }

        private Stock() { }

        public static Stock From(
            DateTime date,
            string symbol,
            decimal open,
            decimal close,
            string label,
            decimal? sma,
            decimal? rsi)
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
    }
}