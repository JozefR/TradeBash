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
                return "5d";
            }
            if (lastDateDifference < TimeSpan.FromDays(30))
            {
                return "1m";
            }
            if (lastDateDifference < TimeSpan.FromDays(90))
            {
                return "3m";
            }
            if (lastDateDifference < TimeSpan.FromDays(180))
            {
                return "6m";
            }
            if (lastDateDifference < TimeSpan.FromDays(360))
            {
                return "1y";
            }
            if (lastDateDifference < TimeSpan.FromDays(720))
            {
                return "2y";
            }
            if (lastDateDifference < TimeSpan.FromDays(1800))
            {
                return "5y";
            }

            return "max";
        }
    }
}