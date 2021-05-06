using System;
using TradeBash.SharedKernel;

namespace TradeBash.Core.Entities.Strategy
{
    public class GeneratedOrder : BaseEntity
    {
        public string Symbol { get; private set; }

        public double OpenPrice { get; private set; }

        public double? ClosePrice { get; private set; }

        public DateTime OpenDate { get; private set; }

        public DateTime? CloseDate { get; private set; }

        public double? ProfitLoss { get; private set; }

        public int BudgetInvestedPercentage { get; private set; }

        public int Position { get; private set; }

        public static GeneratedOrder OpenPosition(
            string symbol,
            double openPrice,
            DateTime openDate)
        {
            var entity = new GeneratedOrder
            {
                Symbol = symbol,
                OpenPrice = openPrice,
                OpenDate = openDate,
            };

            return entity;
        }

        public void NumberOfStocksForPosition(int position)
        {
            Position = position;
        }

        public void NumberOfStocksForPosition(double budget, int openPositions)
        {
            double investition = 0;

            if (openPositions == 0)
            {
                investition = budget * 0.1;
                BudgetInvestedPercentage = 10;
            }

            var numberOfStocks = (int)(investition / OpenPrice);
            Position = numberOfStocks;
        }

        public void ClosePosition(
            double closePrice,
            DateTime closeDate)
        {
            ClosePrice = closePrice;
            CloseDate = closeDate;
        }

        public void CalculateProfitLoss()
        {
            var priceDifference = (ClosePrice - OpenPrice) * Position;
            var roundTwoDecimalPoints = Math.Round((decimal)priceDifference!, 2);
            ProfitLoss = (double)roundTwoDecimalPoints;
        }
    }
}