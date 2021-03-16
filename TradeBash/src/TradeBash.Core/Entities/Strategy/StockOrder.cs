#nullable enable
using System;
using TradeBash.SharedKernel;

namespace TradeBash.Core.Entities
{
    public class StockOrder : BaseEntity
    {
        public DateTime Date { get; private set; }
        
        public string Symbol { get; private set; }
        
        public double Open { get; private set; }
        
        public double Close { get; private set; }
        
        public string Label { get; private set; }

        public double? SMA { get; private set; }

        public double? RSI { get; private set; }

        public string? StrategySignal { get; private set; }

        public double? ProfitLoss { get; private set; }

        public int StrategyId { get; set; }

        private StockOrder() { }

        public static StockOrder From(
            DateTime date,
            string symbol,
            double open,
            double close,
            string label,
            double? sma,
            double? rsi)
        {
            var entity = new StockOrder
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

        public StockOrder FromBackTest(double? profitLoss)
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