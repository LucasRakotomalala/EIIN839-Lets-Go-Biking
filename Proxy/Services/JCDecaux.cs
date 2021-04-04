using Proxy.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Proxy
{
    public class JCDecaux : IJCDecaux
    {
        static readonly HttpClient client = new HttpClient();

        static readonly string URL = "https://api.jcdecaux.com/vls/v2/";
        static readonly string DATA = "stations";
        static readonly string API_KEY = "ff987c28b1313700e2c97651cec164bd6cb4ed76";

        List<Station> IJCDecaux.GetAllStationsFromCity(string city)
        {
            return GetAllStationsFromCity(city).Result;
        }

        private static async Task<List<Station>> GetAllStationsFromCity(string city)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(URL + DATA + "?contract=" + city + "&apiKey=" + API_KEY);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<List<Station>>(responseBody);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return new List<Station>();
            }
        }
    }
}
