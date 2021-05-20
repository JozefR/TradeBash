﻿#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using TradeBash.Core.Entities.Warehouse;
using TradeBash.Core.Services;
using TradeBash.SharedKernel;
using TradeBash.SharedKernel.Interfaces;

namespace TradeBash.Core.Entities.Strategy
{
    public class Strategy : BaseEntity, IAggregateRoot
    {
        public string Name { get; private set; }

        public double Budget { get; private set; }

        public Drawdown _drawdown;

        private double cumulatedBudgetClosePrice;

        private int? _smaShortParameter;
        private int? _smaLongParameter;
        private int? _rsiParameter;

        public ICollection<StrategyStock> StrategyStocksHistory { get; set; }
        public ICollection<GeneratedOrder> GeneratedOrders { get; set; }

        public IReadOnlyCollection<GeneratedOrder> OrderedGeneratedOrdersHistory =>
            GeneratedOrders.OrderBy(x => x.CloseDate).ToList();

        private Strategy()
        {
            Name = string.Empty;
            Budget = double.MinValue;
            StrategyStocksHistory = new List<StrategyStock>();
            GeneratedOrders = new List<GeneratedOrder>();
            _drawdown = new Drawdown();
        }

        public static Strategy From(
            string name,
            double budget,
            int smaShort,
            int rsiParameter)
        {
            var strategy = new Strategy
            {
                Name = name,
                Budget = budget,
                _smaShortParameter = smaShort,
                _rsiParameter = rsiParameter,
            };

            return strategy;
        }

        public static Strategy From(
            string name,
            int budget,
            int? smaShort,
            int? smaLong,
            int rsiParameter)
        {
            var strategy = new Strategy
            {
                Name = name,
                Budget = budget,
                _smaShortParameter = smaShort,
                _smaLongParameter = smaLong,
                _rsiParameter = rsiParameter,
            };

            return strategy;
        }

        public void RunCalculationFor(Stock stock)
        {
            var strategyStock = StrategyStock.From(
                stock.Symbol,
                stock.Name,
                _smaShortParameter,
                _smaLongParameter,
                _rsiParameter);

            foreach (var stockHistory in stock.OrderedHistory)
            {
                strategyStock.CalculateForStock(
                    stock.Symbol,
                    stockHistory.Date,
                    stockHistory.Open,
                    stockHistory.Close,
                    stockHistory.Low);
            }

            StrategyStocksHistory.Add(strategyStock);
        }

        public void RunShortSmaRsi()
        {
            // buy if rsi < 10;
            // sell if sma > 10
            CalculatedStock? generatedSignal = null;
            cumulatedBudgetClosePrice = Budget;
            foreach (var inDate in GetHistoryInDates())
            {
                if (!IsCumulatedCapitalGreaterThen(0)) { break; }

                foreach (var stock in StrategyStocksHistory)
                {
                    var currentStock = GetCurrentStockForDate(stock, inDate);
                    if (currentStock == null) continue;

                    var openPosition = GetCurrentNotClosedPositionsFor(currentStock.Symbol);
                    if (openPosition != null)
                    {
                        ClosePositionForSma(openPosition, currentStock);
                    }

                    if (StrategyGuard.RsiNotCalculated(currentStock)) continue;

                    if (openPosition == null)
                    {
                        if (IsNumberOfAllowedSlotsReached(20)) continue;

                        if (GenerateBuySignalForRsiIfCurrentStockLower(generatedSignal, currentStock, 10))
                        {
                            generatedSignal = currentStock;
                        }
                    }
                }

                if (generatedSignal != null)
                {
                    OpenPositionAndGenerateOrder(generatedSignal, 5);

                    generatedSignal = null;
                }

                Console.WriteLine($"Generate orders for date: {inDate}");
            }
        }

        public void RunShortSmaLongSmaRsi(int rsiValue)
        {
            // buy if rsi < 10;
            // sell if sma > 10
            // only buy when sma long > 200
            CalculatedStock? generatedSignal = null;
            cumulatedBudgetClosePrice = Budget;
            foreach (var inDate in GetHistoryInDates())
            {
                if (!IsCumulatedCapitalGreaterThen(0)) { break; }

                foreach (var stock in StrategyStocksHistory)
                {
                    var currentStock = GetCurrentStockForDate(stock, inDate);

                    if (currentStock == null) continue;

                    var openPosition = GetCurrentNotClosedPositionsFor(currentStock.Symbol);
                    if (openPosition != null)
                    {
                        ClosePositionForSma(openPosition, currentStock);
                    }

                    if (StrategyGuard.LongSmaIsNotCalculated(currentStock)) continue;
                    if (StrategyGuard.LongSmaIsGreaterThenPrice(currentStock)) continue;
                    if (StrategyGuard.RsiNotCalculated(currentStock)) continue;

                    if (openPosition == null)
                    {
                        if (IsNumberOfAllowedSlotsReached(20)) continue;

                        if (GenerateBuySignalForRsiIfCurrentStockLower(generatedSignal, currentStock, rsiValue))
                        {
                            generatedSignal = currentStock;
                        }
                    }
                }

                if (generatedSignal != null)
                {
                    OpenPositionAndGenerateOrder(generatedSignal, 5);

                    generatedSignal = null;
                }

                Console.WriteLine($"Generate orders for date: {inDate}");
            }
        }

        private void ClosePositionForSma(GeneratedOrder position, CalculatedStock currentStock)
        {
            if (currentStock.SMAShort < currentStock.Close)
            {
                var profitLoss = position.ClosePosition(currentStock.Close, currentStock.Date, currentStock.GetIndicatorValues());
                cumulatedBudgetClosePrice += profitLoss;
                _drawdown.Calculate(cumulatedBudgetClosePrice);
                position.SetCumulatedCapitalForClose(cumulatedBudgetClosePrice);
                position.SetMaxDrawdownForClosePrice(_drawdown.TmpDrawDown, Budget);
            }
        }

        private void ClosePosition(GeneratedOrder openPosition, CalculatedStock currentStock)
        {
            var profitLoss = openPosition.ClosePosition(currentStock.Close, currentStock.Date, currentStock.GetIndicatorValues());
            cumulatedBudgetClosePrice += profitLoss;
            _drawdown.Calculate(cumulatedBudgetClosePrice);
            openPosition.SetCumulatedCapitalForClose(cumulatedBudgetClosePrice);
            openPosition.SetMaxDrawdownForClosePrice(_drawdown.TmpDrawDown, Budget);
        }

        private void OpenPositionAndGenerateOrder(CalculatedStock generatedSignal, int budgetPercentage)
        {
            var currentOpenPosition = GetCurrentNotClosedPositionsFor(generatedSignal.Symbol);

            if (currentOpenPosition != null)
            {
                var averageOpenPrice = (currentOpenPosition.OpenPrice + generatedSignal.Close) / 2;
                currentOpenPosition.UpdatePosition(averageOpenPrice, generatedSignal.Date);
                currentOpenPosition.PercentageForStocksFixedMM(Budget, 5);
            }
            else
            {
                var generatedOrder = GeneratedOrder.OpenPosition(
                    generatedSignal.Symbol,
                    generatedSignal.Open,
                    generatedSignal.Date,
                    generatedSignal.GetIndicatorValues());
                generatedOrder.PercentageForStocksFixedMM(Budget, budgetPercentage);
                GeneratedOrders.Add(generatedOrder);
            }
        }

        private bool GenerateBuySignalForRsiIfCurrentStockLower(CalculatedStock? generatedSignal, CalculatedStock currentStock, int rsiThreshold)
        {
            if (currentStock.RSI < rsiThreshold || (generatedSignal != null && currentStock.RSI < generatedSignal.RSI))
            {
                return true;
            }

            return false;
        }

        private GeneratedOrder? GetCurrentNotClosedPositionsFor(string symbol)
        {
            return GeneratedOrders.SingleOrDefault(x => x.CloseDate == null && x.Symbol == symbol);
        }

        private List<DateTime> GetHistoryInDates()
        {
            return StrategyStocksHistory.FirstOrDefault()!.CalculatedOrderedStocksHistory.Select(x => x.Date).ToList();
        }

        private int NumberOfCurrentOpenedPositions(CalculatedStock generatedSignal)
        {
            return GeneratedOrders.Count(x => x.CloseDate == null && x.Symbol == generatedSignal.Symbol);
        }

        private CalculatedStock? GetCurrentStockForDate(StrategyStock strategyStock, DateTime inDate)
        {
            return strategyStock.CalculatedOrderedStocksHistory.FirstOrDefault(x => x.Date == inDate);
        }

        private bool IsNumberOfAllowedSlotsReached(int slots)
        {
            return GeneratedOrders.Count(x => x.CloseDate == null) >= slots;
        }

        private bool IsCumulatedCapitalGreaterThen(int budget)
        {
            return cumulatedBudgetClosePrice >= budget;
        }
    }
}