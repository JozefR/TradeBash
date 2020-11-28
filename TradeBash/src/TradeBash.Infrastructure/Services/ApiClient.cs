using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TradeBash.Infrastructure.Services
{
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializer _jsonSerializer = new JsonSerializer();

        public ApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<JObject>> GetStocksAsync(string urlPath)
        {
            var response = await _httpClient.GetAsync(urlPath);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            return await ReadAsJsonAsync<List<JObject>>(response.Content);
        }

        private static async Task<T> ReadAsJsonAsync<T>(HttpContent httpContent)
        {
            await using (var stream = await httpContent.ReadAsStreamAsync())
            {
                var jsonReader = new JsonTextReader(new StreamReader(stream));

                return _jsonSerializer.Deserialize<T>(jsonReader);
            }
        }
    }
}