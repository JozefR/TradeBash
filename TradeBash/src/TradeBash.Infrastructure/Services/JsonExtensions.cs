using System;
using Newtonsoft.Json.Linq;
using TradeBash.Infrastructure.DTO;

namespace TradeBash.Infrastructure.Services
{
    public static class JsonExtensions
    {
        public static StockResponse MapDataResponse(this JObject jObject, string ticker)
        {
            return new StockResponse()
            {
                Symbol = ticker,
                Date = DateTime.Parse(jObject["date"].ToString()),
                Open = Double.Parse(jObject["open"].ToString()),
                Close = Double.Parse(jObject["close"].ToString()),
                Label = jObject["label"].ToString(),
            };
        }
    }
}