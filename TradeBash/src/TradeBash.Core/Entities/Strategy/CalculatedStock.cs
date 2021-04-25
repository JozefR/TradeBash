using System;
using TradeBash.SharedKernel;

namespace TradeBash.Core.Entities.Strategy
{
    public class CalculatedStock : BaseEntity
    {
        public DateTime Date { get; private set; }

        public double Open { get; private set; }

        public double Close { get; private set; }

        public double? SMA { get; private set; }

        public double? RSI { get; private set; }

        public string? StrategySignal { get; private set; }

        public double? ProfitLoss { get; private set; }

        public static CalculatedStock From(
            DateTime date,
            double open,
            double close,
            double? sma,
            double? rsi)
        {
            var entity = new CalculatedStock
            {
                Date = date,
                Open = open,
                Close = close,
                SMA = sma,
                RSI = rsi
            };

            return entity;
        }

        public CalculatedStock FromBackTest(double? profitLoss)
        {
            ProfitLoss = profitLoss;

            return this;
        }

        public void SetStrategySignal(string? signalFromStrategy)
        {
            StrategySignal = signalFromStrategy;
        }
    }
}