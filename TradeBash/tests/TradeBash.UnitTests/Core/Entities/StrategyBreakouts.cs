using System;
using System.Linq;
using TradeBash.Core.Entities;
using TradeBash.Core.Events;
using Xunit;

namespace TradeBash.UnitTests.Core.Entities
{
    public class StrategyBreakouts
    {
        [Fact]
        public void CalculateBreakoutsStrategy()
        {
            var strategy = Strategy.Set("Breakouts", 5, 14);

            strategy.AddStock(DateTime.Now, "symbol", 1, 1, "label");
            strategy.AddStock(DateTime.Now, "symbol", 1, 2, "label");
            strategy.AddStock(DateTime.Now, "symbol", 1, 3, "label");
            strategy.AddStock(DateTime.Now, "symbol", 1, 4, "label");
            strategy.AddStock(DateTime.Now, "symbol", 1, 5, "label");
            strategy.AddStock(DateTime.Now, "symbol", 1, 6, "label");

            strategy.RunBackTest();

            // strategy statistics
        }
    }
}