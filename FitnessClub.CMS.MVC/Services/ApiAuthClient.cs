using FitnessClub_Test.Dtos;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FitnessClub_Test.CMS.MVC.Services
{
    public class ApiAuthClient
    {
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _http;

        public ApiAuthClient(HttpClient client, IHttpContextAccessor http)
        {
            _client = client;
            _http = http;
        }

        private async Task EnsureTokenAsync()
        {
            var session = _http.HttpContext.Session;
            var token = session.GetString("access");
            var refresh = session.GetString("refresh");

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(refresh))
                return;

            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

            if (jwt.ValidTo > DateTime.UtcNow.AddMinutes(2))
                return;

            var response = await _client.PostAsJsonAsync("user-auth/refresh",
                new { refreshToken = refresh });

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<AuthResponseDTO>(json);

            session.SetString("access", data.Token);
            session.SetString("refresh", data.RefreshToken);
        }

        public async Task<HttpClient> GetAuthorizedClientAsync()
        {
            await EnsureTokenAsync();

            var token = _http.HttpContext.Session.GetString("access");
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            return _client;
        }
    }
}
