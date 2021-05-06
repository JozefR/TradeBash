using System;
using TradeBash.Core.Entities.Strategy;

namespace TradeBash.Core.Entities
{
    public static class StrategyGuard
    {
        public static bool NotSameDate(CalculatedStock currentStock, DateTime inDate)
        {
            if (currentStock.Date != inDate)
            {
                return true;
            }

            return false;
        }

        public static bool IndexOutOfRange(int index, StrategyStock strategyStock)
        {
            if (index >= strategyStock.CalculatedOrderedStocksHistory.Count)
            {
                return true;
            }

            return false;
        }

        public static bool RsiNotCalculated(CalculatedStock calculatedStock)
        {
            if (calculatedStock.RSI == 0)
            {
                return true;
            }

            return false;
        }
    }
}