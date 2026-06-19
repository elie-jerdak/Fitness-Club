using FitnessClub_Test.Dtos;
using FitnessClub_Test.CMS.MVC.Services;
using FitnessClub_Test.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FitnessClub_Test.CMS.MVC.Controllers
{
    public class SignUpController : Controller
    {
        private readonly string _apibaseUrl;

        public SignUpController(IConfiguration configuration)
        {
            _apibaseUrl = configuration["Urls:ApiBaseUrl"];
        }
        public IActionResult Index()
        {
            var dto = new RegisterDTO();
            return View("Index", dto);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            if (!ModelState.IsValid)
                return View("Index", dto);
            
            var json = JsonConvert.SerializeObject(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var client = new HttpClient();
            var response = await client.PostAsync($"{_apibaseUrl}user-auth/Register", content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index", "Login");

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var jsonError = await response.Content.ReadAsStringAsync();
                var apiErrors = JsonConvert.DeserializeObject<SignUpApiErrorResponse>(jsonError);

                foreach (var error in apiErrors.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                return View("Index", dto);
            }

            ModelState.AddModelError(string.Empty, "Registration service is unavailable. Please try again later.");
            return View("Index", dto);
        }
    }

}
