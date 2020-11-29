using System;
using TradeBash.SharedKernel;

namespace TradeBash.Core.Entities
{
    public class Stock : BaseEntity
    {
        public DateTime Date { get; set; }
        
        public string Symbol { get; set; }
        
        public double Open { get; set; }
        
        public double Close { get; set; }
        
        public string Label { get; set; }

        private Stock() { }

        public static Stock From(DateTime date, string symbol, double open, double close, string label)
        {
            var entity = new Stock
            {
                Date = date,
                Symbol = symbol,
                Open = open,
                Close = close,
                Label = label,
            };

            return entity;
        }
    }
}