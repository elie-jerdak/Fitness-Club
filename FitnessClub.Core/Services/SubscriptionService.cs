using FitnessClub_Test.Core.Interfaces;
using FitnessClub_Test.Core.NewModels;
using FitnessClub_Test.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessClub_Test.Core.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly FitnessClubDbContext _context;

        public SubscriptionService(FitnessClubDbContext context)
        {
            _context = context;
        }

        public async Task<InvoiceDTO> RenewSubscriptionService(int membershipId)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var membership = await _context.Memberships
                .Where(m => m.IsActive == true && m.IsDeleted == false)
                .Include(m => m.Client)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == membershipId);
            
            if (membership == null)
                throw new KeyNotFoundException("Membership not found");

            var extensionDays = membership.Type switch
            {
                "Trial" => 7,
                "Monthly" => 30,
                "Yearly" => 365,
                _ => throw new ArgumentOutOfRangeException(nameof(membership.Type), $"Invalid membership type: {membership.Type}")
            };
            membership.StartDate = today;
            membership.EndDate = today.AddDays(extensionDays);
            
            var payment = new SubscriptionPayment
            {
                ClientId = membership.ClientId,
                Amount = GetAmount(membership.Type),
                Date = GetDate(),
                Status = "Suceeded",
                PaymentMethod = "Cash",
                Currency = "USD",
                Type = membership.Type,
                IsActive = true,
                IsDeleted = false,
            };
            _context.SubscriptionPayments.Add(payment);
            await _context.SaveChangesAsync();
            
            var invoiceDto = new InvoiceDTO
            {
                InvoiceID = payment.Id,
                Date = DateTime.UtcNow,
                PaymentMethod = "Cash",
                Currency = "USD",
                User = new UserDTO
                {
                    Id = membership.Client.User.Id,
                    First_Name = membership.Client.User.FirstName,
                    Last_Name = membership.Client.User.LastName,
                    Email = membership.Client.User.Email,
                    Address = membership.Client.User.Address,
                    Phone_Number = membership.Client.User.PhoneNumber
                },
                member = new MemberDTO
                {
                    MemberID = membership.Id,
                    Type = membership.Type,
                    StartDate = membership.StartDate,
                    EndDate = membership.EndDate
                }
            };
            return invoiceDto;
        } 
 
        public async Task<List<ExpiringSubscriptionDto>> GetUpcomingExpirations()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var threshold = today.AddDays(7);

            // Get list of user IDs who have been notified about expiration recently (e.g., last 7 days)
            var notificationLookback = DateTime.UtcNow.AddDays(-7);

            var userNotifiedIds = await _context.Notifications
                .Where(n => n.Type == "expiration" && !n.IsDeleted && n.Time >= notificationLookback)
                .SelectMany(n => n.Users.Select(u => u.Id))
                .Distinct()
                .ToListAsync();

            // Step 2: Fetch EF-translatable data only
            var memberships = await _context.Memberships
                .Where(m => m.EndDate <= threshold &&
                            m.IsAutorenewed == false &&
                            m.Client != null &&
                            m.Client.User != null &&
                            m.Client.User.IsDeleted == false)
                .Select(m => new
                {
                    m.Id,
                    m.Type,
                    m.EndDate,
                    UserId = m.Client.User.Id,
                    FirstName = m.Client.User.FirstName,
                    LastName = m.Client.User.LastName
                })
                .ToListAsync();

            // Step 3: Calculate DaysRemaining and attach NotificationSent info
            var result = memberships
                .Select(m => new ExpiringSubscriptionDto
                {
                    MembershipId = m.Id,
                    UserId = m.UserId,
                    FullName = $"{m.FirstName} {m.LastName}",
                    Membership_Type = m.Type,
                    EndDate = m.EndDate,
                    DaysRemaining = m.EndDate.DayNumber - today.DayNumber,
                    NotificationSent = userNotifiedIds.Contains(m.UserId)
                })
                .OrderBy(dto => dto.DaysRemaining)
                .ToList();

            return result;
        }

        public async Task<InvoiceDTO> GetInvoiceByIdAsync(int invoiceId)
        {
            var invoice = await _context.SubscriptionPayments
                .Where(m => m.IsActive == true && m.IsDeleted == false)
                .Include(sp => sp.Client)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(sp => sp.Id == invoiceId);

            if (invoice == null)
                return null;

            var membership = await _context.Memberships
                .Where(m => m.ClientId == invoice.ClientId)
                .OrderByDescending(m => m.EndDate)
                .FirstOrDefaultAsync();

            if (membership == null)
                throw new Exception("Membership not found for invoice");

            return new InvoiceDTO
            {
                InvoiceID = invoice.Id,
                Date = invoice.Date,
                PaymentMethod = invoice.PaymentMethod,
                Currency = invoice.Currency,
                User = new UserDTO
                {
                    Id = invoice.Client.User.Id,
                    First_Name = invoice.Client.User.FirstName,
                    Last_Name = invoice.Client.User.LastName,
                    Email = invoice.Client.User.Email,
                    Address = invoice.Client.User.Address,
                    Phone_Number = invoice.Client.User.PhoneNumber
                },
                member = new MemberDTO
                {
                    MemberID = membership.Id,
                    Type = membership.Type,
                    StartDate = membership.StartDate,
                    EndDate = membership.EndDate
                }
            };
        

        }

        private decimal GetAmount(string type) => type switch
        {
            "Monthly" => 29.99m,
            "Yearly" => 324.99m,
            "Trial" => 0m,
            _ => throw new Exception("Invalid type")
        };
        private DateTime GetDate()
        {
            return DateTime.UtcNow;
        }
    }
}
