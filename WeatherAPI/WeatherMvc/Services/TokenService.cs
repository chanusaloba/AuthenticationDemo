using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WeatherMvc.Models;
using Newtonsoft.Json;

namespace WeatherMvc.Services
{
    public class TokenService : ITokenService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<TokenService> _logger;
        private readonly IOptions<IdentityServerSettings> _identityServerSettings;
        private readonly DiscoveryDocumentResponse _discoveryDocument;

        public TokenService(ILogger<TokenService> logger, IOptions<IdentityServerSettings> identityServerSettings, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _identityServerSettings = identityServerSettings;
            _clientFactory = clientFactory;

            using var httpClient = new HttpClient();
            _discoveryDocument = httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest {
                Address = identityServerSettings.Value.DiscoveryUrl,
                Policy =
                {
                    RequireHttps = false
                }
            }).Result;
            if (_discoveryDocument.IsError)
            {
                logger.LogError($"Unable to get discovery document. Error is: {_discoveryDocument.Error}");
                throw new Exception("Unable to get discovery document", _discoveryDocument.Exception);
            }
        }

        public async Task<TokenResponse> GetToken(string scope)
        {
            using var client = new HttpClient();
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = _discoveryDocument.TokenEndpoint,

                ClientId = _identityServerSettings.Value.ClientName,
                ClientSecret = _identityServerSettings.Value.ClientPassword,
                Scope = scope
            });


            if (tokenResponse.IsError)
            {
                _logger.LogError($"Unable to get token. Error is: {tokenResponse.Error}");
                throw new Exception("Unable to get token", tokenResponse.Exception);
            }

            return tokenResponse;
        }

        //This one uses the ClientFactory Method in getting the Token
        public async Task<string> GetTokenClientFactoryVersion()
        {
            var client = _clientFactory.CreateClient();

            var values = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "client_id", "m2m.client" },
                { "client_secret", "SuperSecretPassword" },
                { "scope", "weatherapi.read" },
            };

            var content = new FormUrlEncodedContent(values);

            //var response = await client.PostAsync("https://host.docker.internal:44323/connect/token", content);
            var response = await client.PostAsync("https://localhost:44323/connect/token", content);

            var responseContent = await response.Content.ReadAsStringAsync();

            var token = GetTokenValue(responseContent);

            return token;
        }

        private string GetTokenValue(string Content)
        {
            string Token = string.Empty;
            var ResponseObject = JsonConvert.DeserializeObject<ContentResponse>(Content);

            if (ResponseObject != null)
                Token = ResponseObject.access_token;

            return Token;
        }
    }
}
