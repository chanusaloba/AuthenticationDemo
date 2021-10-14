using System.Threading.Tasks;
using System.Net.Http;

namespace WeatherMvc.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly IHttpClientFactory _clientFactory;

        public WeatherService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        //Gets the API Data using ClientFactory Method
        public async Task<HttpResponseMessage> GetWeatherAPIData(string token)
        {
            var client = _clientFactory.CreateClient();

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            //var Result = await client.GetAsync("http://host.docker.internal:44328/weatherforecast");
            var Result = await client.GetAsync("https://localhost:44328/weatherforecast");

            return Result;
        }
    }
}
