using System;

namespace TradeBash.Infrastructure.DTO
{
    public class Stock
    {
        public DateTime Date { get; set; }
        
        public string Symbol { get; set; }
        
        public decimal Open { get; set; }
        
        public decimal Close { get; set; }
        
        public string Label { get; set; }
    }
}