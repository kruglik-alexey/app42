using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace App42
{
    static class Transport
    {
        private const string Host = "http://app42.azurewebsites.net/";

        public static async Task<T> Get<T>(string path)
        {
            var data = await new HttpClient().GetStringAsync($"{Host}/{path}");
            return JsonConvert.DeserializeObject<T>(data);
        }

        public static async Task<T> Post<T>(string path, T data = null) where T : class
        {            
            string content = "";
            if (data != null)
            {
                var jsonSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                };
                content = JsonConvert.SerializeObject(data, jsonSettings);
            }
            var responce = await new HttpClient().PostAsync($"{Host}/{path}", new StringContent(content, Encoding.UTF8, "application/json"));
            responce.EnsureSuccessStatusCode();
            var responceData = await responce.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responceData);
        }

        public static async Task Post(string path) => await Post<object>(path);
    }
}