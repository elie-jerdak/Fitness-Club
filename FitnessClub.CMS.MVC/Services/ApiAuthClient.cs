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
            var session = _http.HttpContext?.Session;

            if (session == null)
                return;

            var token = session.GetString("access");
            var refresh = session.GetString("refresh");

            if (string.IsNullOrWhiteSpace(token) ||
                string.IsNullOrWhiteSpace(refresh))
                return;

            JwtSecurityToken jwt;

            try
            {
                jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            }
            catch
            {
                session.Clear();
                return;
            }

            // Token still valid
            if (jwt.ValidTo > DateTime.UtcNow.AddMinutes(2))
                return;

            Console.WriteLine($"Inside ApiAuthClient calling refresh {_client.BaseAddress}");

            var response = await _client.PostAsJsonAsync(
                "/user-auth/refresh",
                new { refreshToken = refresh });

            // Refresh failed
            if (!response.IsSuccessStatusCode)
            {
                session.Clear();
                return;
            }

            var json = await response.Content.ReadAsStringAsync();

            var data = JsonConvert.DeserializeObject<AuthResponseDTO>(json);

            // Invalid response
            if (data == null ||
                string.IsNullOrWhiteSpace(data.Token) ||
                string.IsNullOrWhiteSpace(data.RefreshToken))
            {
                session.Clear();
                return;
            }

            session.SetString("access", data.Token);
            session.SetString("refresh", data.RefreshToken);
        }

        public async Task<HttpClient> GetAuthorizedClientAsync()
        {
            await EnsureTokenAsync();

            var token = _http.HttpContext?.Session.GetString("access");

            if (!string.IsNullOrWhiteSpace(token))
            {
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            return _client;
        }
    }
}