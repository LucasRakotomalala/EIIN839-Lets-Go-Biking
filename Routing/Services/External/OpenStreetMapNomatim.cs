using Routing.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Routing.Services.External
{
    public class OpenStreetMapNomatim
    {
        private static readonly HttpClient client = new HttpClient();

        public OpenStreetMapNomatim()
        {
            // Nothing to do
        }

        public async Task<List<Place>> GetPlaces(string request)
        {
            try
            {
                Console.WriteLine("[{0}] Requête GET vers OpenStreetMap pour avoir le point le plus près d'une adresse avec l'URL : {1}\n", DateTime.Now, request);
                HttpResponseMessage response = await client.GetAsync(request);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<List<Place>>(responseBody);
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public async Task<List<Place>> GetPlacesFromAddress(string address)
        {
            string request = "https://nominatim.openstreetmap.org/search?email=lucas.rakotomalala@etu.univ-cotedazur.fr&format=json&q=" + address;
            return await GetPlaces(request);
        }
    }
}
