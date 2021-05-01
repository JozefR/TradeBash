using System;
using System.Linq;
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
                Close = jObject.TryGetValue("close", out JToken close) ? Double.Parse(close.ToString()) : 0,
                High = jObject.TryGetValue("high", out JToken high) ? Double.Parse(high.ToString()) : 0,
                Low = jObject.TryGetValue("low", out JToken low) ? Double.Parse(low.ToString()) : 0,
                Open = jObject.TryGetValue("open", out JToken open) ? Double.Parse(open.ToString()) : 0,
                Symbol = jObject.TryGetValue("symbol", out JToken symbol) ? symbol.ToString() : string.Empty,
                Volume = jObject.TryGetValue("volume", out JToken volume) ? Double.Parse(volume.ToString()) : 0,
                Date = jObject.TryGetValue("date", out JToken date) ? DateTime.Parse(date.ToString()) : DateTime.MinValue,
                ChangeOverTime = jObject.TryGetValue("changeOverTime", out JToken changeOverTime) ? double.Parse(changeOverTime.ToString()) : 0,
                MarketChangeOverTime = jObject.TryGetValue("marketChangeOverTime", out JToken marketChangeOverTime) ? double.Parse(marketChangeOverTime.ToString()) : 0,
                UOpen = jObject.TryGetValue("uOpen", out JToken uOpen) ? double.Parse(uOpen.ToString()) : 0,
                UClose = jObject.TryGetValue("uClose", out JToken uClose) ? double.Parse(uClose.ToString()) : 0,
                UHigh = jObject.TryGetValue("uHigh", out JToken uHigh) ? double.Parse(uHigh.ToString()) : 0,
                ULow = jObject.TryGetValue("uLow", out JToken uLow) ? double.Parse(uLow.ToString()) : 0,
                UVolume = jObject.TryGetValue("uVolume", out JToken uVolume) ? double.Parse(uVolume.ToString()) : 0,
                FOpen = jObject.TryGetValue("fOpen", out JToken fOpen) ? double.Parse(fOpen.ToString()) : 0,
                FClose = jObject.TryGetValue("fClose", out JToken fClose) ? double.Parse(fClose.ToString()) : 0,
                FHigh = jObject.TryGetValue("fHigh", out JToken fHigh) ? double.Parse(fHigh.ToString()) : 0,
                FLow = jObject.TryGetValue("fLow", out JToken fLow) ? double.Parse(fLow.ToString()) : 0,
                FVolume = jObject.TryGetValue("fVolume", out JToken fVolume) ? double.Parse(fVolume.ToString()) : 0,
                Change = jObject.TryGetValue("change", out JToken change) ? double.Parse(change.ToString()) : 0,
                ChangePercent = jObject.TryGetValue("changePercent", out JToken changePercent) ? double.Parse(changePercent.ToString()) : 0,
                Label = jObject.TryGetValue("label", out JToken label) ? label.ToString() : string.Empty,
            };
        }
    }
}