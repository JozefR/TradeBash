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
        private readonly IConfiguration _configuration;
        private readonly IApiClient _apiClient;

        public StocksController(IRepository repository, IConfiguration configuration, IApiClient apiClient)
        {
            _repository = repository;
            _configuration = configuration;
            _apiClient = apiClient;
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
            var path = _configuration.GetConnectionString("IEXConnection");
            string iexPath = String.Format(path, String.Concat(ticker), String.Concat(history));

            var items = await _apiClient.GetStocksAsync(iexPath);

            return Ok(items);
        }
    }
}