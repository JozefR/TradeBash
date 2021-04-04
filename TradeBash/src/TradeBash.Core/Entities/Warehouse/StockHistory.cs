using System;
using TradeBash.SharedKernel;

namespace TradeBash.Core.Entities.Warehouse
{
    public class StockHistory : ValueObject
    {
        public double Close { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Open { get; set; }
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

        private StockHistory(){}

        public static StockHistory From(
            DateTime date,
            double open,
            double close,
            string label,
            double high,
            double low,
            double volume,
            double changeOverTime,
            double marketChangeOverTime,
            double uOpen,
            double uClose,
            double uHigh,
            double uLow,
            double uVolume,
            double fOpen,
            double fClose,
            double fHigh,
            double fLow,
            double fVolume,
            double change,
            double changePercent)
        {
            var entity = new StockHistory
            {
                Date = date,
                Open = open,
                Close = close,
                Label = label,
                High = high,
                Low = low,
                Volume = volume,
                ChangeOverTime = changeOverTime,
                MarketChangeOverTime = marketChangeOverTime,
                UOpen = uOpen,
                UClose = uClose,
                UHigh = uHigh,
                ULow = uLow,
                UVolume = uVolume,
                FOpen = fOpen,
                FClose = fClose,
                FHigh = fHigh,
                FLow = fLow,
                FVolume = fVolume,
                Change = change,
                ChangePercent = changePercent,
            };

            return entity;
        }
    }
}