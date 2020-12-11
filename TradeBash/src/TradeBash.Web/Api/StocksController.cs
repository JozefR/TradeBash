using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TradeBash.Core.Entities;
using TradeBash.Infrastructure.Services;
using TradeBash.SharedKernel.Interfaces;
using TradeBash.Web.ApiModels;

namespace TradeBash.Web.Api
{
    public class StocksController : BaseApiController
    {
        private readonly IRepository _repository;
        private readonly IApiClient _apiClient;
        private readonly string IexPath;

        public StocksController(
            IRepository repository, 
            IConfiguration configuration, 
            IApiClient apiClient)
        {
            _repository = repository;
            _apiClient = apiClient;
            
            IexPath = configuration.GetConnectionString("IEXConnection");
        }
        
        // GET: api/StockItems
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var items = (await _repository.ListAsync<Stock>())
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
        
        [HttpPatch("iex/populate/{ticker}/{history}")]
        public async Task<IActionResult> PopulateDb(string ticker, string history)
        {
            string iexPath = String.Format(IexPath, String.Concat(ticker), String.Concat(history));
            
            var items = await _apiClient.GetStocksAsync(iexPath);

            var data = items.Select(x => x.MapDataResponse(ticker));

            var strategy = Strategy.SetIndicatorParameters(5, 2);

            foreach (var stockResponse in data)
            {
                strategy.AddStock(
                    stockResponse.Date,
                    stockResponse.Symbol,
                    stockResponse.Open,
                    stockResponse.Close,
                    stockResponse.Label);
            }

            await _repository.AddAsync(strategy);

            return Ok();
        }
    }
}