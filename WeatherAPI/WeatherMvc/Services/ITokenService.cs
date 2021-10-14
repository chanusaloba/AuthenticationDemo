using System.Threading.Tasks;
using IdentityModel.Client;
using System.Net.Http;

namespace WeatherMvc.Services
{
    public interface ITokenService
    {
        Task<TokenResponse> GetToken(string scope);
        Task<string> GetTokenClientFactoryVersion();
    }
}
