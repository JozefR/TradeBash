using System;
using System.Linq;
using TradeBash.Core.Entities;
using Xunit;

namespace TradeBash.UnitTests.Core.Entities
{
    public class StrategySmaIndicator
    {
        [Fact]
        public void CalculateSimpleMovingAverageForStock()
        {
            var item = Strategy.CalculateForStock(5, 14);

            item.AddStock(DateTime.Now, "symbol", 1, 1, "label");
            item.AddStock(DateTime.Now, "symbol", 1, 2, "label");
            item.AddStock(DateTime.Now, "symbol", 1, 3, "label");
            item.AddStock(DateTime.Now, "symbol", 1, 4, "label");
            item.AddStock(DateTime.Now, "symbol", 1, 5, "label");
            item.AddStock(DateTime.Now, "symbol", 1, 6, "label");

           var result = item.StocksHistory.Last().SMA;
           Assert.Equal(3, result);
        }
    }
}