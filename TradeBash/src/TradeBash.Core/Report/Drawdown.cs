using System.Linq;
using TradeBash.Core.Entities.Strategy;

namespace TradeBash.Core.Report
{
    public class Drawdown : IDrawdown
    {
        public double Peak { get; set; }
        public double Trough { get; set; }
        public double MaxDrawDown { get; set; }

        public double MaxDrawDownPercentage { get; set; }

        public Drawdown()
        {
            Peak = 0;
            Trough = 0;
            MaxDrawDown = 0;
        }

        public void Calculate(Strategy strategy)
        {
            Peak = 0;
            Trough = 0;
            MaxDrawDown = 0;

            int i = 0;
            foreach (var order in strategy.OrderedGeneratedOrdersHistory)
            {
                var cumulatedCapital = order.CumulatedCapital;
                if (cumulatedCapital == 0) continue;
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
                    {
                        MaxDrawDown = tmpDrawDown;
                        MaxDrawDownPercentage = (MaxDrawDown / strategy.OrderedGeneratedOrdersHistory.Take(i).Max(x => x.CumulatedCapital)) * 100;
                    }
                }

                i++;
            }
        }

        public double GetMaxDrawdown()
        {
            return MaxDrawDown;
        }

        public double GetMaxDrawdownPercentage()
        {
            return MaxDrawDownPercentage;
        }
    }
}