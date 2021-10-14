using System.Threading.Tasks;
using System.Net.Http;

namespace WeatherMvc.Services
{
    public interface IWeatherService
    {
        Task<HttpResponseMessage> GetWeatherAPIData(string token);
    }
}
