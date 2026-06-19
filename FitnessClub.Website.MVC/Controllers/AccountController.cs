using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FitnessClub_Test.Dtos;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using Microsoft.AspNetCore.Http;
using YourProjectNamespace.Models;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using FitnessClub_Test.Core.DTOs;
using FitnessClub.Website.MVC.Models;

public class AccountController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _apiBaseUrl;


    public AccountController(IHttpClientFactory httpClientFactory, IConfiguration config)
    {
        _httpClientFactory = httpClientFactory;
        _apiBaseUrl = config["ApiBaseUrl"];
    }

    [HttpGet]
    public IActionResult Register()
    {
        ViewBag.Genders = new SelectList(new List<SelectListItem> {
        new SelectListItem { Text = "Select Gender", Value = "", Selected = true },
        new SelectListItem { Text = "Male", Value = "M" },
        new SelectListItem { Text = "Female", Value = "F" }
        }, "Value", "Text");

        ViewBag.Roles = new SelectList(new List<SelectListItem>{
        new SelectListItem { Text = "Select Role", Value = "", Selected = true },
        new SelectListItem { Text = "Client", Value = "Client" },
        new SelectListItem { Text = "Coach", Value = "Coach" }
        }, "Value", "Text");
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Genders = new SelectList(new List<SelectListItem> {
        new SelectListItem { Text = "Select Gender", Value = "", Selected = true },
        new SelectListItem { Text = "Male", Value = "M" },
        new SelectListItem { Text = "Female", Value = "F" }
        }, "Value", "Text");

            ViewBag.Roles = new SelectList(new List<SelectListItem>{
        new SelectListItem { Text = "Select Role", Value = "", Selected = true },
        new SelectListItem { Text = "Client", Value = "Client" },
        new SelectListItem { Text = "Coach", Value = "Coach" }
        }, "Value", "Text");
            return View();
        }

        // Map to DTO
        var dto = new RegisterDTO
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            Password = model.Password,
            Dob = model.Dob,
            Gender = model.Gender,
            PhoneNumber = model.PhoneNumber,
        };

        // Call API
        var client = _httpClientFactory.CreateClient("FitnessApi");
        client.BaseAddress = new Uri("http://localhost:5118");
        var json = JsonSerializer.Serialize(dto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("api/auth/register", content);

        if (response.IsSuccessStatusCode)
        {
            TempData["RegisterSuccess"] = "Registration successful! Please log in.";
            TempData["ToastMessage"] = "Signed up successfully! Please log in.";
            TempData["ToastType"] = "success";

            return RedirectToAction("Register");
        }

        TempData["RegisterError"] = "Registration failed. Please try again.";
        TempData["ToastMessage"] = "Registration failed. Please try again.";
        TempData["ToastType"] = "danger";
        ViewBag.Genders = new SelectList(new List<SelectListItem> {
        new SelectListItem { Text = "Select Gender", Value = "", Selected = true },
        new SelectListItem { Text = "Male", Value = "M" },
        new SelectListItem { Text = "Female", Value = "F" },
        new SelectListItem { Text = "Other", Value = "O" }
        }, "Value", "Text");

        ViewBag.Roles = new SelectList(new List<SelectListItem>{
        new SelectListItem { Text = "Select Role", Value = "", Selected = true },
        new SelectListItem { Text = "Client", Value = "Client" },
        new SelectListItem { Text = "Coach", Value = "Coach" }
        }, "Value", "Text");
        return View();
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View("Register");
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        var client = _httpClientFactory.CreateClient();
        var loginDto = new LoginDTO
        {
            Email = model.Email,
            Password = model.Password
        };
        var response = await client.PostAsJsonAsync("http://localhost:5118/api/auth/login", loginDto);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
            HttpContext.Session.SetString("JWToken", result.Token);
            TempData["ToastMessage"] = "Logged in successfully!";
            TempData["ToastType"] = "success";
            // Redirect to home or dashboard
            return RedirectToAction("Index", "Home");
        }
        TempData["ToastMessage"] = "Login failed: " + response.RequestMessage;
        TempData["ToastType"] = "danger";
        ModelState.AddModelError("", "Invalid login attempt");
        return View();
    }

    public async Task<IActionResult> Profile()
    {
        var token = HttpContext.Session.GetString("JWToken");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login");
        }

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("http://localhost:5118/api/auth/me");

        if (!response.IsSuccessStatusCode)
        {
            return RedirectToAction("Login");
        }

        var profile = await response.Content.ReadFromJsonAsync<UserProfileDto>();
        return View(profile);
    }


    [HttpGet("/checkinout")]
    public async Task<IActionResult> CheckInOut(int userId)
    {
        var client = _httpClientFactory.CreateClient("FitnessApi");


        var response = await client.PostAsJsonAsync("api/check-in-out", userId);

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Message = "Failed to check in/out! Please try again.";
            return View("CheckInOut");
        }

        var result = await response.Content.ReadFromJsonAsync<CheckingInOutDto>();

        ViewBag.Message = result.TimeOut == null
            ? $"Checked In at {result.TimeIn:t}"
            : $"Checked Out at {result.TimeOut:t}";

        return View("CheckInOut");
    }
    public IActionResult Logout()
    {
        HttpContext.Session.Remove("JWToken");
        return RedirectToAction("Login", "Account");
    }

    [HttpGet]
    public async Task<IActionResult> UpdateProfile()
    {
        var token = HttpContext.Session.GetString("JWToken");
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login");

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("http://localhost:5118/api/auth/me");
        if (!response.IsSuccessStatusCode)
            return RedirectToAction("Login");

        var profile = await response.Content.ReadFromJsonAsync<UserProfileDto>();

        var updateModel = new UpdateProfileViewModel
        {
            FirstName = profile.FirstName,
            SecondName = profile.SecondName,
            PhoneNumber = profile.PhoneNumber,
            Role = profile.Role,
            Weight = profile.Weight,
            Height = profile.Height,
            MedicalIssues = profile.MedicalIssues,
            Target = profile.Target,
            Salary = profile.Salary,
            Experience = profile.Experience,
            Specialty = profile.Specialty
        };

        return View(updateModel);
    }


    [HttpPost]
    public async Task<IActionResult> UpdateProfile(UpdateProfileViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var token = HttpContext.Session.GetString("JWToken");
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login");

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var dto = new UserProfileUpdateDto
        {
            FirstName = model.FirstName,
            SecondName = model.SecondName,
            PhoneNumber = model.PhoneNumber,
            Role = model.Role,
            Weight = model.Weight,
            Height = model.Height,
            MedicalIssues = model.MedicalIssues,
            Target = model.Target,
            Salary = model.Salary,
            Experience = model.Experience,
            Specialty = model.Specialty
        };

        var response = await client.PutAsJsonAsync("http://localhost:5118/api/auth/update", dto);

        if (response.IsSuccessStatusCode)
        {
            TempData["ToastMessage"] = "Profile updated successfully!";
            TempData["ToastType"] = "success";
            return RedirectToAction("Profile");
        }
        else
        {
            TempData["ToastMessage"] = "Update failed, please try again.";
            TempData["ToastType"] = "danger";
            return View(model);
        }
    }


}

public class TokenResponse
{
    public string Token { get; set; }
}


