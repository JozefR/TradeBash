using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace TradeBash.Infrastructure.Services
{
    public interface IApiClient
    { 
        Task<List<JObject>> GetStocksAsync(string urlPath);
    }
}