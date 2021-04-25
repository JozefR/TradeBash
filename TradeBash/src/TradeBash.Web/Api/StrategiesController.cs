using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TradeBash.Core.Entities.Strategy;
using TradeBash.Core.Entities.Warehouse;
using TradeBash.Infrastructure.Services;
using TradeBash.SharedKernel.Interfaces;

namespace TradeBash.Web.Api
{
    public class StrategiesController : BaseApiController
    {
        private readonly IRepository _repository;
        private readonly IApiClient _apiClient;

        public StrategiesController(
            IRepository repository,
            IApiClient apiClient)
        {
            _repository = repository;
            _apiClient = apiClient;
        }

        [HttpGet("breakouts")]
        public async Task<Strategy> CalculateBreakouts(string ticker, string history)
        {
            var stocks = await _repository.ListAsync<Stock>();

            var strategy = Strategy.From("Test", 5, 2);

            strategy.RunCalculation(stocks);

            await _repository.AddAsync(strategy);

            return strategy;
        }
    }
}