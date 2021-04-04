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
            var items = (await _stockRepository.ListAsync<StockOrder>())
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

            var data = items.Select(x => x.MapDataResponse());

            var strategy = Strategy.Set("test", 5, 2);
            foreach (var stockResponse in data)
            {
                strategy.AddStock(
                    stockResponse.Date,
                    stockResponse.Symbol,
                    stockResponse.Open,
                    stockResponse.Close,
                    stockResponse.Label);
            }

            await _stockRepository.AddAsync(strategy);

            return Ok();
        }

        [HttpPatch("iex/populate/stocks/{ticker}/{history}")]
        public async Task<IActionResult> PopulateStocks(string ticker, string history)
        {
            var path = @"C:\Users\Jozef.Randjak\source\repos\git\TradeBash\TradeBash\src\TradeBash.Web\sp100.txt";

            var stocksToDownload = new List<Tuple<string, string>>();
            using (TextFieldParser csvParser = new TextFieldParser(path))
            {
                csvParser.SetDelimiters(",");
                csvParser.HasFieldsEnclosedInQuotes = true;

                // Skip the row with the column names
                csvParser.ReadLine();

                while (!csvParser.EndOfData)
                {
                    // Read current line fields, pointer moves to the next line.
                    string[] fields = csvParser.ReadFields();
                    string symbol = fields[0];
                    string name = fields[1];
                    stocksToDownload.Add(new Tuple<string, string>(symbol, name));
                }
            }

            /*foreach (var stockToDownload in stocksToDownload)
            {
                var symbol = stockToDownload.Item1;
                var name = stockToDownload.Item2;

                var stockExist = await _stockRepository.GetBySymbolAsync(symbol);
                if (stockExist == null)
                {
                    var iexPath = String.Format(IexPath, String.Concat(symbol), String.Concat(history));
                    var items = await _apiClient.GetStocksAsync(iexPath);

                    var data = items.Select(x => x.MapDataResponse());

                    var stock = Stock.From(symbol, name);
                    foreach (var stockResponse in data)
                    {
                        stock.AddHistory(
                            stockResponse.Date,
                            stockResponse.Open,
                            stockResponse.Close,
                            stockResponse.Label);
                    }

                    await _stockRepository.AddAsync(stock);
                }
            }*/

            return Ok();
        }
    }
}