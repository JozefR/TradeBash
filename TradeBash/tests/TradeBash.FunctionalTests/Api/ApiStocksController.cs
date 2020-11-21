using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SQLitePCL;
using TradeBash.Core.Entities;
using TradeBash.Web;
using TradeBash.Web.ApiModels;
using Xunit;

namespace TradeBash.FunctionalTests.Api
{
    public class ApiStocksController : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        public ApiStocksController(CustomWebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
        }
        
        [Fact]
        public async Task ReturnsTwoItems()
        {
            var response = await _client.GetAsync("/api/stocks");
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<IEnumerable<StockDTO>>(stringResponse).ToList();

            Assert.Equal(2, result.Count());
            Assert.Contains(result, i => i.Symbol == SeedData.Stock1.Symbol);
            Assert.Contains(result, i => i.Symbol == SeedData.Stock2.Symbol);
        }
    }
}