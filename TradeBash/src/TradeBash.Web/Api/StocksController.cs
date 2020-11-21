using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TradeBash.Core.Entities;
using TradeBash.SharedKernel.Interfaces;
using TradeBash.Web.ApiModels;

namespace TradeBash.Web.Api
{
    public class StocksController : BaseApiController
    {
        private readonly IRepository _repository;

        public StocksController(IRepository repository)
        {
            _repository = repository;
        }
        
        // GET: api/StockItems
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var items = (await _repository.ListAsync<Stock>())
                .Select(StockDTO.From);
            
            return Ok(items);
        }
    }
}