using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TradeBash.Infrastructure.Services;
using TradeBash.SharedKernel.Interfaces;

namespace TradeBash.Web.Api
{
    public class StrategiesController : BaseApiController
    {
        private readonly IRepository _repository;
        private readonly IApiClient _apiClient;
        private readonly string IexPath;

        public StrategiesController(
            IRepository repository,
            IConfiguration configuration,
            IApiClient apiClient)
        {
            _repository = repository;
            _apiClient = apiClient;

            IexPath = configuration.GetConnectionString("IEXConnection");
        }

        [HttpGet("breakouts/{ticker}/{history}")]
        public async Task<IActionResult> CalculateBreakouts(string ticker, string history)
        {
            return null;
        }
    }
}