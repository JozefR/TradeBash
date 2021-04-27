using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TradeBash.Core.Entities.Strategy;
using TradeBash.Core.Entities.Warehouse;
using TradeBash.Infrastructure.Data.Repositories;
using TradeBash.SharedKernel.Interfaces;

namespace TradeBash.Web.Api
{
    public class StrategiesController : BaseApiController
    {
        private readonly IRepository _repository;
        private readonly IStrategyRepository _strategyRepository;

        public StrategiesController(IRepository repository, IStrategyRepository strategyRepository)
        {
            _repository = repository;
            _strategyRepository = strategyRepository;
        }

        [HttpGet("calculateIndicators/{strategyName}/{budget}/{smaParameter}/{rsiParameter}")]
        public async Task<OkResult> CalculateIndicators(string strategyName, double budget, int smaParameter, int rsiParameter)
        {
            var strategy = Strategy.From(strategyName, budget, smaParameter, rsiParameter);

            var stocks = await _repository.ListAsync<Stock>();
            strategy.RunCalculation(stocks);

            await _repository.AddAsync(strategy);

            return Ok();
        }

        [HttpGet("Backtest/{strategyName}")]
        public async Task<OkResult> Backtest(string strategyName)
        {
            var strategy = await _strategyRepository.GetByNameAsync(strategyName);

            strategy.RunBackTest();

            await _repository.UpdateAsync(strategy);

            return Ok();
        }
    }
}