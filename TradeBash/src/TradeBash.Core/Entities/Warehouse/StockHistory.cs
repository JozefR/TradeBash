using System;
using TradeBash.SharedKernel;

namespace TradeBash.Core.Entities.Warehouse
{
    public class StockHistory : ValueObject
    {
        public DateTime Date { get; private set; }

        public double Open { get; private set; }

        public double Close { get; private set; }

        public string Label { get; private set; }

        private StockHistory() { }

        public static StockHistory From(
            DateTime date,
            double open,
            double close,
            string label)
        {
            var entity = new StockHistory
            {
                Date = date,
                Open = open,
                Close = close,
                Label = label,
            };

            return entity;
        }
    }
}