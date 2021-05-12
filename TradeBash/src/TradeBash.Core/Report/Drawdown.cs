using TradeBash.Core.Entities.Strategy;

namespace TradeBash.Core.Report
{
    public class Drawdown : IDrawdown
    {
        public double Peak { get; set; }
        public double Trough { get; set; }
        public double MaxDrawDown { get; set; }

        private double StrategyBudget;

        public Drawdown()
        {
            Peak = 0;
            Trough = 0;
            MaxDrawDown = 0;
        }

        public void Calculate(Strategy strategy)
        {
            StrategyBudget = strategy.Budget;

            foreach (var order in strategy.OrderedGeneratedOrdersHistory)
            {
                var cumulatedCapital = order.CumulatedCapital;
                if (cumulatedCapital > Peak)
                {
                    Peak = cumulatedCapital;
                    Trough = Peak;
                }
                else if (cumulatedCapital < Trough)
                {
                    Trough = cumulatedCapital;
                    var tmpDrawDown = Peak - Trough;
                    if (tmpDrawDown > MaxDrawDown)
                        MaxDrawDown = tmpDrawDown;
                }
            }
        }

        public double GetMaxDrawdown()
        {
            return MaxDrawDown;
        }

        public double GetMaxDrawdownPercentage()
        {
            return (MaxDrawDown / StrategyBudget) * 100;
        }
    }
}