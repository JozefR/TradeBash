using TradeBash.Core.Entities.Strategy;

namespace TradeBash.Core.Report
{
    public interface IDrawdown
    {
        void Calculate(Strategy strategy);
        double GetMaxDrawdown();
        double GetMaxDrawdownPercentage();
    }
}