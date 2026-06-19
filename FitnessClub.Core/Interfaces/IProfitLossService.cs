using FitnessClub_Test.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace FitnessClub_Test.Core.Interfaces
{
    public interface IProfitLossService
    {
        RevenueChartDTO GetMonthlyRevenue(int monthsCount = 3);

        RevenueChartDTO GetMonthlyExpenses(int monthsCount = 3);

        FinanceTableDto GetFinanceTableData(int monthsCount = 3);
    }
}