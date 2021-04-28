using Proxy.Models;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Routing.Services.External
{
    class ProxyService
    {
        private static readonly HttpClient client = new HttpClient();

        public ProxyService()
        {
            // Nothing to do
        }

        public async Task<JCDecauxItem> GetJCDecauxItem(string request)
        {
            try
            {
                Console.WriteLine("[{0}] Requête GET vers le Proxy avec l'URL : {1}\n", DateTime.Now, request);
                HttpResponseMessage response = await client.GetAsync(request);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<JCDecauxItem>(responseBody);
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public async Task<JCDecauxItem> GetJCDecauxItem(string contract_name, string stationNumber)
        {
            string request = "http://localhost:8081/api/jcdecaux/station?city=" + contract_name + "&number=" + stationNumber;
            return await GetJCDecauxItem(request);
        }
    }
}
