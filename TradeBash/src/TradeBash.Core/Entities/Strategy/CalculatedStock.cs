using System;
using TradeBash.SharedKernel;

namespace TradeBash.Core.Entities.Strategy
{
    public class CalculatedStock : BaseEntity
    {
        public string Symbol { get; private set; }

        public DateTime Date { get; private set; }

        public double Open { get; private set; }

        public double Close { get; private set; }

        public double Low { get; private set; }

        public double? SMAShort { get; private set; }

        public double? SMALong { get; private set; }

        public double? RSI { get; private set; }

        public static CalculatedStock From(
            string symbol,
            DateTime date,
            double open,
            double close,
            double low)
        {
            var entity = new CalculatedStock
            {
                Symbol = symbol,
                Date = date,
                Open = open,
                Close = close,
                Low = low
            };

            return entity;
        }

        public void SetIndicators(double? smaShort, double? smaLong, double? rsi)
        {
            SMAShort = smaShort;
            SMALong = smaLong;
            RSI = rsi;
        }
    }
}