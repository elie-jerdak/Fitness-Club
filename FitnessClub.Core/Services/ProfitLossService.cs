using FitnessClub_Test.Core.Interfaces;
using FitnessClub_Test.Core.NewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessClub_Test.Dtos;

namespace FitnessClub_Test.Core.Services
{
    public class ProfitLossService : IProfitLossService
    {
        private readonly FitnessClubDbContext _context;

        public ProfitLossService(FitnessClubDbContext context)
        {
            _context = context;
        }

        public RevenueChartDTO GetMonthlyRevenue(int monthsCount = 3)
        {
            var now = DateTime.UtcNow;
            var months = Enumerable.Range(0, monthsCount)
                .Select(i => now.AddMonths(-i))
                .OrderBy(m => m)
                .ToList();

            var categories = months.Select(m => m.ToString("MMM")).ToList();

            var membershipFees = months.Select(m =>
                _context.SubscriptionPayments
                    .Where(p => p.Date.Month == m.Month && p.Date.Year == m.Year && !p.IsDeleted)
                    .Sum(p => (decimal?)p.Amount) ?? 0
            ).ToList();

            var classFees = months.Select(m =>
                (from booking in _context.Bookings
                 join c in _context.Classes on booking.ClassId equals c.Id
                 where booking.Date.HasValue &&
                       booking.Date.Value.Month == m.Month &&
                       booking.Date.Value.Year == m.Year &&
                       !booking.IsDeleted &&
                       !c.IsDeleted
                 select (decimal?)c.Fee
                ).Sum() ?? 0
            ).ToList();

            ////////////add the premade program fee//////////////

            var privateBookingRevenue = months.Select(m =>
            {
                var totalPrivateBookings = _context.AvailabilityBookings
                    .Where(b =>
                        b.Date.Month == m.Month &&
                        b.Date.Year == m.Year &&
                        !b.IsDeleted)
                    .Count();

                decimal bookingFee = 150m; // replace with real fee if dynamic (Add Fee to each Availability)
                decimal gymCommissionRate = 0.20m;

                var totalRevenue = totalPrivateBookings * bookingFee;
                var gymRevenue = totalRevenue * gymCommissionRate;

                return gymRevenue;
            }).ToList();

            var merchandiseSales = new List<decimal> { 1151, 2270, 1997 };

            return new RevenueChartDTO
            {
                Categories = categories,
                Series = new List<RevenueSeriesDto>
            {
                new RevenueSeriesDto { Name = "Membership Fees", Data = membershipFees },
                new RevenueSeriesDto { Name = "Class Fees", Data = classFees },
                new RevenueSeriesDto { Name = "Private Booking Commissions", Data = privateBookingRevenue },
                new RevenueSeriesDto { Name = "Merchandise Sales", Data = merchandiseSales}
            }
            };
        }

        public RevenueChartDTO GetMonthlyExpenses(int monthsCount = 3)
        {
            var now = DateTime.UtcNow;
            var months = Enumerable.Range(0, monthsCount)
                .Select(i => now.AddMonths(-i))
                .OrderBy(m => m)
                .ToList();

            var categories = months.Select(m => m.ToString("MMM")).ToList();

            // Total monthly salary cost from all active coaches
            decimal totalMonthlySalary = (
                from coach in _context.Coaches
                join user in _context.Users on coach.UserId equals user.Id
                where user.IsDeleted != true
                select (decimal?)coach.Salary
            ).Sum() ?? 0;

            var salariesFees = Enumerable.Repeat(totalMonthlySalary, monthsCount).ToList();

            var maintenance = new List<decimal> { 1251, 2570, 1697 };
            
            var utilities = new List<decimal> { 121, 230, 1297 };

            return new RevenueChartDTO
            {
                Categories = categories,
                Series = new List<RevenueSeriesDto>
            {
                new RevenueSeriesDto { Name = "Salaries Fees", Data = salariesFees },
                new RevenueSeriesDto { Name = "Maintenance Fees", Data = maintenance },
                new RevenueSeriesDto { Name = "Utilitiy Sales", Data = utilities }
            }
            };
        }

        public FinanceTableDto GetFinanceTableData(int monthsCount = 3)
        {
            var revenue = GetMonthlyRevenue(monthsCount);
            var expenses = GetMonthlyExpenses(monthsCount);

            return new FinanceTableDto
            {
                Months = revenue.Categories,
                Revenues = revenue.Series.ToDictionary(s => s.Name, s => s.Data),
                Expenses = expenses.Series.ToDictionary(s => s.Name, s => s.Data)
            };
        }
    }
}
