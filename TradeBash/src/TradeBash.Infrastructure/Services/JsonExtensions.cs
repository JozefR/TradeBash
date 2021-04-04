using System;
using Newtonsoft.Json.Linq;
using TradeBash.Infrastructure.DTO;

namespace TradeBash.Infrastructure.Services
{
    public static class JsonExtensions
    {
        public static StockDtoResponse MapDataResponse(this JObject jObject)
        {
            return new StockDtoResponse()
            {
                Close = Double.Parse(jObject["close"].ToString()),
                High = Double.Parse(jObject["high"].ToString()),
                Low = Double.Parse(jObject["low"].ToString()),
                Open = Double.Parse(jObject["open"].ToString()),
                Symbol = jObject["symbol"].ToString(),
                Volume = Double.Parse(jObject["volume"].ToString()),
                Date = DateTime.Parse(jObject["date"].ToString()),
                ChangeOverTime = Double.Parse(jObject["changeOverTime"].ToString()),
                MarketChangeOverTime = Double.Parse(jObject["marketChangeOverTime"].ToString()),
                UOpen = Double.Parse(jObject["uOpen"].ToString()),
                UClose = Double.Parse(jObject["uClose"].ToString()),
                UHigh = Double.Parse(jObject["uHigh"].ToString()),
                ULow = Double.Parse(jObject["uLow"].ToString()),
                UVolume = Double.Parse(jObject["uVolume"].ToString()),
                FOpen = Double.Parse(jObject["fOpen"].ToString()),
                FClose = Double.Parse(jObject["fClose"].ToString()),
                FHigh = Double.Parse(jObject["fHigh"].ToString()),
                FLow = Double.Parse(jObject["fLow"].ToString()),
                FVolume = Double.Parse(jObject["fVolume"].ToString()),
                Change = Double.Parse(jObject["change"].ToString()),
                ChangePercent = Double.Parse(jObject["changePercent"].ToString()),
                Label = jObject["label"].ToString(),
            };
        }
    }
}