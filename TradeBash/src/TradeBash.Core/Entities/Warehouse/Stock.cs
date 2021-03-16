using System;
using System.Collections.Generic;
using TradeBash.SharedKernel;

namespace TradeBash.Core.Entities.Warehouse
{
    public class Stock : BaseEntity
    {
        public string Symbol { get; private set; }

        public IReadOnlyCollection<StockHistory> History => _history;

        private readonly List<StockHistory> _history;

        private Stock()
        {
            _history = new List<StockHistory>();
        }

        public static Stock From(string symbol)
        {
            var entity = new Stock
            {
                Symbol = symbol,
            };

            return entity;
        }

        public StockHistory AddHistory(
            DateTime date,
            double open,
            double close,
            string label)
        {
            var stock = StockHistory.From(date, open, close, label);
            _history.Add(stock);

            return stock;
        }
    }
}