using System;
using TradeBash.SharedKernel;

namespace TradeBash.Core.Entities.Strategy
{
    public class GeneratedOrder : BaseEntity
    {
        public string Symbol { get; private set; }

        public string Ticker { get; private set; }

        public double OpenPrice { get; private set; }

        public double? ClosePrice { get; private set; }

        public DateTime OpenDate { get; private set; }

        public DateTime? CloseDate { get; private set; }

        public double? ProfitLoss { get; private set; }

        public int Position { get; set; }

        public static GeneratedOrder OpenPosition(
            double openPrice,
            DateTime openDate,
            int position)
        {
            var entity = new GeneratedOrder
            {
                OpenPrice = openPrice,
                OpenDate = openDate,
                Position = position,
            };

            return entity;
        }

        public void ClosePosition(
            double closePrice,
            DateTime closeDate)
        {
            ClosePrice = closePrice;
            CloseDate = closeDate;
        }
    }
}