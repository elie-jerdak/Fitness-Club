using FitnessClub_Test.Core.Interfaces;
using FitnessClub_Test.Dtos;
using FitnessClub_Test.CMS.MVC.Services;
using FitnessClub_Test.Core.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FitnessClub_Test.CMS.MVC.Controllers
{
    public class DashboardController : BaseController
    {

        private readonly IConfiguration _configuration;
        private readonly IProfitLossService _profitLossService;
        private readonly ApiAuthClient _api;

        public DashboardController(IConfiguration configuration, ApiAuthClient api,
            IProfitLossService profitLossService) : base(api)
        {
            _api = api;
            _configuration = configuration;
            _profitLossService = profitLossService;
        }

        [Authorize(Roles = "Admin, Coach")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var token = HttpContext.Session.GetString("access");
                if (string.IsNullOrEmpty(token))
                    return null;

                var role = JwtHelper.GetRole(token);
                if (role != "Admin")
                {
                    return Redirect(_configuration["Urls:Website"]);
                }

                // Fetch Latest Members here
                var client = await _api.GetAuthorizedClientAsync();
                
                var members = await client.GetFromJsonAsync<List<MemberDTO>>("members/get-members");
                int totalMembersCount = members?.Count ?? 0;

                var cutoffDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-14));

                var filteredLatestMembers = members?
                    .Where(m => m.StartDate >= cutoffDate)
                    .OrderByDescending(m => m.StartDate)
                    .Take(8)
                    .Select(m => new LatestMembersDTO
                    {
                        First_Name = m.First_Name,
                        Photo = m.Photo,
                        StartDate = m.StartDate
                    })
                    .ToList() ?? new List<LatestMembersDTO>();

                var takenCount = filteredLatestMembers.Count;

                var upexpirations = await client.GetFromJsonAsync<List<ExpiringSubscriptionDto>>("upcoming-expiration/upcoming");
                int numberOfExpirations = upexpirations?.Count ?? 0;

                var dashboardDto = new DashboardDTO
                {
                    latestMembers = filteredLatestMembers,
                    latestMembersCount = takenCount,
                    TotalMembersCount = totalMembersCount,
                    TotalNumberUpcomingExpirations = numberOfExpirations,
                    Revenue = GetCurrentMonthRevenue(),
                    Expenses = GetCurrentMonthExpenses()
                };

                return View("Dashboard", dashboardDto);


            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in: ", ex.Message);

                return View("Dashboard", new DashboardDTO());

            }

        }

        private decimal GetCurrentMonthRevenue()
        {
            var currentMonthRevenueDto = _profitLossService.GetMonthlyRevenue(1);

            decimal membershipFee = currentMonthRevenueDto.Series
                .FirstOrDefault(s => s.Name == "Membership Fees")?.Data.FirstOrDefault() ?? 0;

            decimal classFee = currentMonthRevenueDto.Series
                .FirstOrDefault(s => s.Name == "Class Fees")?.Data.FirstOrDefault() ?? 0;

            decimal merchandiseSales = currentMonthRevenueDto.Series
                .FirstOrDefault(s => s.Name == "Merchandise Sales")?.Data.FirstOrDefault() ?? 0;

            decimal totalRevenue = membershipFee + classFee + merchandiseSales;
            return totalRevenue;
        }
        private decimal GetCurrentMonthExpenses()
        {
            var currentMonthExpenses = _profitLossService.GetMonthlyExpenses(1);

            decimal salaries = currentMonthExpenses.Series
                .FirstOrDefault(s => s.Name == "Salaries Fees")?.Data.FirstOrDefault() ?? 0;

            decimal maintenance = currentMonthExpenses.Series
                .FirstOrDefault(s => s.Name == "Maintenance Fees")?.Data.FirstOrDefault() ?? 0;

            decimal utilities = currentMonthExpenses.Series
                .FirstOrDefault(s => s.Name == "Utilitiy Sales")?.Data.FirstOrDefault() ?? 0;

            return salaries + maintenance + utilities;
        }
        
    }
}
