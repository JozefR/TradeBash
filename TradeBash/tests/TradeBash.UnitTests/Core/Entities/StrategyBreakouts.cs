﻿using System;
using System.Linq;
using FluentAssertions;
using TradeBash.Core.Entities;
using TradeBash.Core.Entities.Strategy;
using Xunit;

namespace TradeBash.UnitTests.Core.Entities
{
    public class StrategyBreakouts
    {
        /*[Fact(Skip = "not implemented")]
        public void Price_Crossed_MovingAverage_From_Bottom_To_Up_ShouldBuy()
        {
            // arrange
            var strategy = Strategy.From("Breakout", 10);
            strategy.CalculateForStock(DateTime.Now, "symbol", 1, 1, "label");
            strategy.CalculateForStock(DateTime.Now, "symbol", 1, 2, "label");
            strategy.CalculateForStock(DateTime.Now, "symbol", 1, 3, "label");
            strategy.CalculateForStock(DateTime.Now, "symbol", 1, 4, "label");
            strategy.CalculateForStock(DateTime.Now, "symbol", 1, 5, "label");
            strategy.CalculateForStock(DateTime.Now, "symbol", 1, 6, "label");

            // act
            strategy.RunBackTest();

            // assert
            strategy.StocksHistory.Should().Contain(x => x.StrategySignal.Contains("Buy"));
        }

        [Fact(Skip = "not implemented")]
        public void Price_Crossed_MovingAverage_From_Top_To_Bottom_ShouldSell()
        {
            // arrange
            var strategy = Strategy.From("Breakout", 10);
            strategy.CalculateForStock(DateTime.Now, "symbol", 1, 1, "label");
            strategy.CalculateForStock(DateTime.Now, "symbol", 1, 2, "label");
            strategy.CalculateForStock(DateTime.Now, "symbol", 1, 3, "label");
            strategy.CalculateForStock(DateTime.Now, "symbol", 1, 4, "label");
            strategy.CalculateForStock(DateTime.Now, "symbol", 1, 5, "label");
            strategy.CalculateForStock(DateTime.Now, "symbol", 1, 6, "label");

            // act
            strategy.RunBackTest();

            // assert
            strategy.StocksHistory.Should().Contain(x => x.StrategySignal.Contains("Sell"));
        }*/
    }
}