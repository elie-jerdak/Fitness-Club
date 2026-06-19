using FitnessClub_Test.Dtos;
using FitnessClub_Test.CMS.MVC.Services;
using FitnessClub_Test.Core.Helpers;
using FitnessClub_Test.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FitnessClub_Test.CMS.MVC.Controllers
{
    
    public class LoginController : Controller
    {
        private readonly ApiAuthClient _api;
        private readonly string _apiBaseUrl;
        private readonly IConfiguration _configuration;
        private readonly IMvcAuthService _mvcAuthService;

        public LoginController(ApiAuthClient api, IConfiguration configuration, IMvcAuthService mvcAuthService)
        {
            _api = api;
            _configuration = configuration;
            _apiBaseUrl = configuration["Urls:ApiBaseUrl"];
            _mvcAuthService = mvcAuthService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO user)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View("Index", user);
            }

            var client = new HttpClient();
            var response = await client.PostAsJsonAsync($"{_apiBaseUrl}user-auth/Login", user);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View("Index", user);
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<AuthResponseDTO>(json);

            // Store token and refreshToken
            HttpContext.Session.SetString("access", result.Token);
            HttpContext.Session.SetString("refresh", result.RefreshToken);

            // Create MVC identity cookie
            await _mvcAuthService.SignInAsync(result.Token);

            // Read role directly from token
            var role = JwtHelper.GetRole(result.Token);

            // Redirect based on role
            if (role == "Admin")
                return RedirectToAction("Index", "Dashboard");
            
            return Redirect(_configuration["Urls:Website"]); 
        }

        public async Task<IActionResult> Logout()
        {
            var client = await _api.GetAuthorizedClientAsync();
            await client.PostAsync($"{_apiBaseUrl}user-auth/logout", null);
            
            await _mvcAuthService.SignOutAsync();
            HttpContext.Session.Clear();
            
            return RedirectToAction("Index", "Login");
        }

        public IActionResult Forbidden()
        {
            return View();
        }

        public IActionResult Bridge()
        {
            var token = HttpContext.Session.GetString("access");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            return View(model: token);
        }
    }
}
