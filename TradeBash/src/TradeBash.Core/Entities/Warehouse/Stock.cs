using System;
using System.Collections.Generic;
using System.Linq;
using TradeBash.SharedKernel;

namespace TradeBash.Core.Entities.Warehouse
{
    public class Stock : BaseEntity
    {
        public string Symbol { get; private set; }

        public string Name { get; private set; }

        private readonly List<StockHistory> _history;

        public IReadOnlyCollection<StockHistory> OrderedHistory => _history.OrderBy(x => x.Date).ToList();

        private Stock()
        {
            _history = new List<StockHistory>();
        }

        public static Stock From(string symbol, string name)
        {
            var entity = new Stock
            {
                Symbol = symbol,
                Name = name
            };

            return entity;
        }

        public StockHistory AddHistory(
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
            var stock = StockHistory.From(
                date,
                open,
                close,
                label,
                high,
                low,
                volume,
                changeOverTime,
                marketChangeOverTime,
                uOpen,
                uClose,
                uHigh,
                uLow,
                uVolume,
                fOpen,
                fClose,
                fHigh,
                fLow,
                fVolume,
                change,
                changePercent);

            _history.Add(stock);

            return stock;
        }
    }
}