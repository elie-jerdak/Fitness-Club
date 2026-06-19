using FitnessClub_Test.Dtos;
using FitnessClub_Test.CMS.MVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FitnessClub_Test.CMS.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class Analytics_ReportsController : BaseController
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly ILogger<Analytics_ReportsController> _logger;
        private readonly ApiAuthClient _api;

        public Analytics_ReportsController(ApiAuthClient api, ILogger<Analytics_ReportsController> logger) : base(api)
        {
            _api = api;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var client = await _api.GetAuthorizedClientAsync();
                var response = await client.GetFromJsonAsync<FinanceTableDto>("report-charts/financeTable");
                return View("Profit_LossView", response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Getting Finance Table Data");
                return View("Profit_LossView", new FinanceTableDto());
            }
        }

        public async Task<IActionResult> ClassUtilizationRate()
        {
            try
            {
                var client = await _api.GetAuthorizedClientAsync();
                var response = await client.GetFromJsonAsync<List<ClassUtilizationDTO>>("report-charts/ClassUtilizationRate");

                return View("ClassUtilizationRateView", response); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Getting Class Utilization Rate Data");
                return View("ClassUtilizationRateView", new List<ClassUtilizationDTO>());
            }
        }

    }
}
