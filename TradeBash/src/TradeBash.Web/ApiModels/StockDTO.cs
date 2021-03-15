using System;
using TradeBash.Core.Entities;

namespace TradeBash.Web.ApiModels
{
    public class StockDTO
    {
        public int Id { get; set; }
        
        public DateTime Date { get; set; }

        public string Symbol { get; set; }
        
        public double Open { get; set; }
        
        public double Close { get; set; }
        
        public string Label { get; set; }
        
        public static StockDTO From(StockOrder item)
        {
            return new StockDTO
            {
                Id = item.Id,
                Date = item.Date,
                Symbol = item.Symbol,
                Open = item.Open,
                Close = item.Close,
                Label = item.Label,
            };
        }
    }
}