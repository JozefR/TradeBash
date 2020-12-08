using System;

namespace TradeBash.Infrastructure.DTO
{
    public class Stock
    {
        public DateTime Date { get; set; }
        
        public string Symbol { get; set; }
        
        public double Open { get; set; }
        
        public double Close { get; set; }
        
        public string Label { get; set; }
    }
}