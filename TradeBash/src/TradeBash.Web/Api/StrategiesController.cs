using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TradeBash.Core.Entities.Strategy;
using TradeBash.Core.Entities.Warehouse;
using TradeBash.Infrastructure.Data.Repositories;
using TradeBash.Infrastructure.Services;
using TradeBash.SharedKernel.Interfaces;

namespace TradeBash.Web.Api
{
    public class StrategiesController : BaseApiController
    {
        private readonly IRepository _repository;
        private readonly IStrategyRepository _strategyRepository;
        private readonly IApiClient _apiClient;

        public StrategiesController(
            IRepository repository,
            IApiClient apiClient,
            IStrategyRepository strategyRepository)
        {
            _repository = repository;
            _apiClient = apiClient;
            _strategyRepository = strategyRepository;
        }

        [HttpGet("calculateIndicators")]
        public async Task<Strategy> CalculateIndicators(string ticker, string history)
        {
            var stocks = await _repository.ListAsync<Stock>();

            var strategy = Strategy.From("Test", 5, 2);

            strategy.RunCalculation(stocks);

            await _repository.AddAsync(strategy);

            return strategy;
        }

        [HttpGet("Backtest")]
        public async Task<Strategy> Backtest()
        {
            var strategy = await _strategyRepository.GetByIdAsync(1);

            strategy.RunBackTest();

            await _repository.UpdateAsync(strategy);

            return strategy;
        }
    }
}