using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic.FileIO;
using TradeBash.Core.Entities.Strategy;
using TradeBash.Core.Entities.Warehouse;
using TradeBash.Infrastructure.Data.Repositories;
using TradeBash.Infrastructure.Services;
using TradeBash.Web.ApiModels;

namespace TradeBash.Web.Api
{
    public class StocksController : BaseApiController
    {
        private readonly IStockRepository _stockRepository;
        private readonly IApiClient _apiClient;
        private readonly string IexPath;

        public StocksController(
            IStockRepository stockRepository,
            IConfiguration configuration, 
            IApiClient apiClient)
        {
            _stockRepository = stockRepository;
            _apiClient = apiClient;
            
            IexPath = configuration.GetConnectionString("IEXConnection");
        }
        
        // GET: api/StockItems
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var items = (await _stockRepository.ListAsync<StrategyStock>())
                .Select(StockDTO.From);
            
            return Ok(items);
        }
        
        [HttpGet("iex/{ticker}/{history}")]
        public async Task<IActionResult> GetStock(string ticker, string history)
        {
            string iexPath = String.Format(IexPath, String.Concat(ticker), String.Concat(history));

            var items = await _apiClient.GetStocksAsync(iexPath);

            return Ok(items);
        }   
    }
}