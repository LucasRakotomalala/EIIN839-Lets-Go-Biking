using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace Proxy.Models
{
    [DataContract]
    public class JCDecauxItem
    {
        private static readonly string URL = "https://api.jcdecaux.com/vls/v2/";
        private static readonly string DATA = "stations";
        private static readonly string API_KEY = "ff987c28b1313700e2c97651cec164bd6cb4ed76";

        private static readonly HttpClient client = new HttpClient();

        private string request;
        [DataMember]
        public List<Station> stations { get; set; }

        public JCDecauxItem()
        {
            request = URL + DATA + "?apiKey=" + API_KEY;
            stations = CallAPI(request).Result;
        }

        public JCDecauxItem(Dictionary<string, string> dictionary)
        {
            request = URL + DATA + "?contract=" + dictionary["city"] + "&apiKey=" + API_KEY;
            stations = CallAPI(request).Result;
        }

        private static async Task<List<Station>> CallAPI(string request)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(request);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<List<Station>>(responseBody);
            }
            catch (HttpRequestException)
            {
                return new List<Station>();
            }
        }
    }
    [DataContract]
    public class Station
    {
        [DataMember]
        public string contract_name { get; set; }

        [DataMember]
        public string name { get; set; }

        [DataMember]
        public string address { get; set; }

        [DataMember]
        public int number { get; set; }

        [DataMember]
        public Position position { get; set; }

        [DataMember]
        public int bike_stands { get; set; }

        [DataMember]
        public int available_bike_stands { get; set; }

        [DataMember]
        public int available_bikes { get; set; }

        [DataMember]
        public string status { get; set; }

        public override string ToString()
        {
            return
                "Contract Name: " + contract_name + "\n" +
                "Address: " + address + "\n" +
                "Name: " + name + "\n" +
                "Number: " + number + "\n" +
                position.ToString() +
                "Bike Stands: " + bike_stands + "\n" +
                "Available Bike Stands: " + available_bike_stands + "\n" +
                "Available Bikes: " + available_bikes + "\n" +
                "Status: " + status + "\n";
        }
    }

    [DataContract]
    public class Position
    {
        [DataMember]
        public double latitude { get; set; }

        [DataMember]
        public double longitude { get; set; }

        public override string ToString()
        {
            return
                "Latitude: " + latitude + "\n" +
                "Longitude: " + longitude + "\n";
        }
    }
}