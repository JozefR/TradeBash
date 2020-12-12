using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TradeBash.Core.Entities;
using TradeBash.Web;
using Xunit;

namespace TradeBash.FunctionalTests.Api
{
    public class ApiStrategiesController : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        public ApiStrategiesController(CustomWebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task ReturnsBreakoutsStrategy()
        {
            var response = await _client.GetAsync("/api/strategies/breakouts/{aapl}/{1y}");
            response.EnsureSuccessStatusCode();

            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<IEnumerable<Strategy>>(stringResponse).ToList();

            Assert.NotNull(result);
        }
    }
}