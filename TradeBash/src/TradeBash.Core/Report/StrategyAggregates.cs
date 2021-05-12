using System.Linq;
using TradeBash.Core.Entities.Strategy;

namespace TradeBash.Core.Report
{
    public static class StrategyAggregates
    {
        public static double GetPercentageWinners(Strategy strategy)
        {
            double winnerOrders = strategy.GeneratedOrders.Count(x => x.ProfitLoss > 0);
            double allTrades = strategy.GeneratedOrders.Count;
            double winnersPercentage = (winnerOrders / allTrades) * 100;
            return winnersPercentage;
        }

        public static string GetTestedHistory(Strategy strategy)
        {
            var minDate = strategy.GeneratedOrders.Min(x => x.OpenDate);
            var maxDate = strategy.GeneratedOrders.Max(x => x.CloseDate);
            var testedHistory = $"{minDate.ToShortDateString()} - {maxDate.Value.ToShortDateString()}";
            return testedHistory;
        }

        public static double GetEndingCapital(Strategy strategy)
        {
            return strategy.OrderedGeneratedOrdersHistory.Last().CumulatedCapital;
        }

        public static double GetNettProfit(Strategy strategy)
        {
            return (double)strategy.OrderedGeneratedOrdersHistory.Sum(x => x.ProfitLoss);
        }

        public static double GetProfitFactor(Strategy strategy)
        {
            var profitTrades = strategy.GeneratedOrders.Where(x => x.ProfitLoss > 0).Sum(x => x.ProfitLoss);
            var lossTrades = strategy.GeneratedOrders.Where(x => x.ProfitLoss < 0).Sum(x => x.ProfitLoss);
            var profitFactor = profitTrades / lossTrades;
            return (double)profitFactor;
        }

        public static int GetNumberOfTrades(Strategy strategy)
        {
            return strategy.GeneratedOrders.Count;
        }
    }
}