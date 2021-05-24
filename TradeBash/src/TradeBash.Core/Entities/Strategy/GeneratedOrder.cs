using System;
using TradeBash.SharedKernel;

namespace TradeBash.Core.Entities.Strategy
{
    public class GeneratedOrder : BaseEntity
    {
        public string Symbol { get; private set; }

        public double OpenPrice { get; private set; }

        public string OpenIndicators { get; private set; }

        public double? ClosePrice { get; private set; }

        public string? CloseIndicators { get; private set; }

        public DateTime OpenDate { get; private set; }

        public DateTime? CloseDate { get; private set; }

        public double? ProfitLoss { get; private set; }

        public double CumulatedCapital { get; private set; }

        public double DrawdownPercentage { get; private set; }

        public int BudgetInvestedPercentage { get; private set; }

        public int Position { get; private set; }

        public string AdditionallyBoughtPositions { get; private set; }

        public static GeneratedOrder OpenPosition(
            string symbol,
            double openPrice,
            DateTime openDate,
            string openIndicators)
        {
            var entity = new GeneratedOrder
            {
                Symbol = symbol,
                OpenPrice = openPrice,
                OpenDate = openDate,
                OpenIndicators = openIndicators
            };

            return entity;
        }

        public void UpdateOpenPrice(GeneratedOrder currentOpenPosition, CalculatedStock generatedSignal)
        {
            var averageOpenPrice = (currentOpenPosition.OpenPrice + generatedSignal.Close) / 2;
            OpenPrice = averageOpenPrice;
            AdditionallyBoughtPositions += generatedSignal.Date.ToShortDateString() + ", ";
        }

        public void UpdateCurrentPositions(double budget, int percentage)
        {
            BudgetInvestedPercentage += percentage;

            int numberOfStocks = CalculateNumberOfStocksForOpenPrice(budget, percentage);

            Position += numberOfStocks;
        }

        public void NumberOfStocksForPosition(double budget, int openPositions)
        {
            double investment = 0;

            if (openPositions == 0)
            {
                investment = budget * 0.1;
                BudgetInvestedPercentage = 10;
            }

            var numberOfStocks = (int)(investment / OpenPrice);
            Position = numberOfStocks;
        }

        public double ClosePosition(double closePrice, DateTime closeDate, string indicatorValues)
        {
            ClosePrice = closePrice;
            CloseDate = closeDate;
            CloseIndicators = indicatorValues;

            var priceDifference = (ClosePrice - OpenPrice) * Position;
            var roundTwoDecimalPoints = Math.Round((decimal)priceDifference!, 2);
            ProfitLoss = (double)roundTwoDecimalPoints;
            return (double) ProfitLoss;
        }

        public double CalculateIntradayProfitLoss(double lowPrice)
        {
            var priceDifference = (lowPrice - OpenPrice) * Position;
            var roundTwoDecimalPoints = Math.Round((decimal)priceDifference!, 2);
            ProfitLoss = (double)roundTwoDecimalPoints;
            return (double) ProfitLoss;
        }

        public void SetCumulatedCapitalForClose(double budget)
        {
            CumulatedCapital = budget;
        }

        public void SetMaxDrawdownForClosePrice(double drawdown, double strategyBudget)
        {
            DrawdownPercentage = -Math.Round((drawdown / strategyBudget) * 100, 2);
        }

        private int CalculateNumberOfStocksForOpenPrice(double budget, int percentage)
        {
            double percentageDivider = (double)percentage / 100;
            var investment = budget * percentageDivider;
            var numberOfStocks = (int)(investment / OpenPrice);
            return numberOfStocks;
        }
    }
}