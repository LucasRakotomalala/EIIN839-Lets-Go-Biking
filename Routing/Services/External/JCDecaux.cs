using Proxy.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Routing.Services.External
{
    class JCDecaux
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly string API_KEY = "ff987c28b1313700e2c97651cec164bd6cb4ed76";

        private readonly string request = "https://api.jcdecaux.com/vls/v2/stations?apiKey=" + API_KEY;

        public JCDecaux()
        {
            // Nothing to do
        }

        public async Task<List<Station>> GetStations()
        {
            try
            {
                Console.WriteLine("[{0}] Requête GET vers JCDecaux pour obtenir toutes les stations avec l'URL : {1}\n", DateTime.Now, request);
                HttpResponseMessage response = await client.GetAsync(request);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<List<Station>>(responseBody);
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }
    }
}
