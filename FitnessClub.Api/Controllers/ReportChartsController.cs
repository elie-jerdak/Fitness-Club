using FitnessClub_Test.Core.Interfaces;
using FitnessClub_Test.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace FitnessClub_Test.Api.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ReportChartsController : ControllerBase
    {
        private readonly IProfitLossService _profitLoss;
        private readonly IClassUtilizationService _classUtilizationService;

        public ReportChartsController(IProfitLossService profitLoss, IClassUtilizationService classutilizationService)
        {
            _profitLoss = profitLoss;
            _classUtilizationService = classutilizationService;
        }


        [HttpGet("monthlyRevenue")]
        public IActionResult GetMonthlyRevenue()
        {
            var data = _profitLoss.GetMonthlyRevenue();
            return Ok(data);
        }

        [HttpGet("monthlyExpenses")]
        public IActionResult GetMonthlyExpenses()
        {
            var data = _profitLoss.GetMonthlyExpenses();
            return Ok(data);
        }

        [HttpGet("financeTable")]
        public ActionResult<FinanceTableDto> GetFinanceTable(int monthsCount = 3)
        {
            var result = _profitLoss.GetFinanceTableData(monthsCount);
            return Ok(result);
        }

        [HttpGet("ClassUtilizationRate")]
        public ActionResult<List<ClassUtilizationDTO>> GetClassUtilizationRate()
        {
            var result = _classUtilizationService.ClassUtilization();
            return Ok(result);
        }
    }
}
