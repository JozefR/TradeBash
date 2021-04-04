using System;

namespace TradeBash.Infrastructure.DTO
{
    public class StockDto
    {
        public double Close { get; set; }

        public double High { get; set; }

        public double Low { get; set; }

        public double Open { get; set; }

        public string Symbol { get; set; }

        public double Volume { get; set; }

        public DateTime Date { get; set; }

        public double ChangeOverTime { get; set; }

        public double MarketChangeOverTime { get; set; }

        public double UOpen { get; set; }

        public double UClose { get; set; }

        public double UHigh { get; set; }

        public double ULow { get; set; }

        public double UVolume { get; set; }

        public double FOpen { get; set; }

        public double FClose { get; set; }

        public double FHigh { get; set; }

        public double FLow { get; set; }

        public double FVolume { get; set; }

        public string Label { get; set; }

        public double Change { get; set; }

        public double ChangePercent { get; set; }
    }
}