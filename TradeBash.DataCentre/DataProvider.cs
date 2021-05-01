using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TradeBash.Infrastructure.DTO;
using TradeBash.Infrastructure.Services;

namespace TradeBash.DataCentre
{
    interface IDataProvider
    {
        Task<List<StockDtoResponse>> GetSerializedStocksFromDataProviderAsync(string symbol, string range);
        string GetRangeForHistoricalData(TimeSpan lastDateDifference);
    }

    public class DataProvider : IDataProvider
    {
        private readonly IApiClient _apiClient;
        private readonly string _iexPath;

        public DataProvider(IConfiguration configuration, IApiClient apiClient)
        {
            _apiClient = apiClient;
            _iexPath = configuration.GetConnectionString("IEXConnection");
        }

        public async Task<List<StockDtoResponse>> GetSerializedStocksFromDataProviderAsync(string symbol, string range)
        {
            var constructedPath = string.Format(_iexPath, symbol, range);
            var stocksHistory = await _apiClient.GetStocksAsync(constructedPath);
            var stocksHistorySerialized = stocksHistory.Select(JsonExtensions.MapDataResponse);
            return stocksHistorySerialized.ToList();
        }

        public string GetRangeForHistoricalData(TimeSpan lastDateDifference)
        {
            if (lastDateDifference < TimeSpan.FromDays(5))
            {
                return HistoryRange.FiveDays;
            }

            if (lastDateDifference < TimeSpan.FromDays(30))
            {
                return HistoryRange.OneMonth;
            }

            if (lastDateDifference < TimeSpan.FromDays(90))
            {
                return HistoryRange.ThreeMonths;
            }

            if (lastDateDifference < TimeSpan.FromDays(180))
            {
                return HistoryRange.SixMonth;
            }

            if (lastDateDifference < TimeSpan.FromDays(360))
            {
                return HistoryRange.OneYear;
            }

            if (lastDateDifference < TimeSpan.FromDays(720))
            {
                return HistoryRange.TwoYears;
            }

            if (lastDateDifference < TimeSpan.FromDays(1800))
            {
                return HistoryRange.FiveYears;
            }

            return HistoryRange.Max;
        }
    }
    
    
    public static class HistoryRange
    {
        public static string FiveDays = "5d";
        public static string OneMonth = "1m";
        public static string ThreeMonths = "3m";
        public static string SixMonth = "6m";
        public static string OneYear = "1y";
        public static string TwoYears = "2y";
        public static string FiveYears = "5y";
        public static string Max = "max";
    }
}