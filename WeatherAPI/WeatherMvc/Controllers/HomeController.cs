using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WeatherMvc.Models;
using WeatherMvc.Services;

namespace WeatherMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITokenService _tokenService;
        private readonly IWeatherService _weatherService;

        public HomeController(ILogger<HomeController> logger, ITokenService tokenService, IWeatherService weatherService)
        {
            _logger = logger;
            _tokenService = tokenService;
            _weatherService = weatherService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> Weather()
        {
            var data = new List<WeatherData>();


            using (var client = new HttpClient())
            {

                //var tokenResponse = await _tokenService.GetToken("weatherapi.read");

                //client.SetBearerToken(tokenResponse.AccessToken);

                //var result = await client
                //  .GetAsync("http://host.docker.internal:44328/weatherforecast");

                //Uses Client Factory Method
                var token = await _tokenService.GetTokenClientFactoryVersion();

                var result = await _weatherService.GetWeatherAPIData(token);

                if (result.IsSuccessStatusCode)
                {
                    var model = result.Content.ReadAsStringAsync().Result;

                    data = JsonConvert.DeserializeObject<List<WeatherData>>(model);

                    return View(data);
                }
                else
                {
                    throw new Exception("Unable to get content: ");
                }
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
