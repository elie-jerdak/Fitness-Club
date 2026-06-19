using FitnessClub_Test.Core.Interfaces;
using FitnessClub_Test.Core.NewModels;
using FitnessClub_Test.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessClub_Test.Core.Services
{
    public class NotificationService : INotificationService
    {
        private readonly FitnessClubDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(FitnessClubDbContext context, IEmailService emailService, ILogger<NotificationService> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<List<Notification>> GenerateExpirationNotifications()
        {
            var dateThreshold = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7));
            var notifications = new List<Notification>();
            var now = DateOnly.FromDateTime(DateTime.UtcNow);

            var expiringSubs = await _context.Memberships
                .Where(a => a.EndDate <= dateThreshold && a.EndDate > now && a.IsDeleted == false && a.IsActive == true)
                .Include(s => s.Client)
                    .ThenInclude(u => u.User)
                .ToListAsync();
            
            foreach (var sub in expiringSubs)
            {
                var daysUntilExpiration = sub.EndDate.Day - now.Day;
                //used if automatic sending notifications
                if (new[] { 7, 3, 1 }.Contains(daysUntilExpiration))
                {
                    var notification = new Notification
                    {
                        Type = "expiration",
                        Message = $"Subscription expires in {daysUntilExpiration} day(s)",
                        Time = DateTime.UtcNow,
                        IsGlobal = false,
                        IsDeleted = false
                    };

                    if (sub.Client?.User != null)
                    {
                        notification.Users.Add(sub.Client.User);
                    }

                    _context.Notifications.Add(notification);
                    notifications.Add(notification);
                }
            }

            await _context.SaveChangesAsync();
            return notifications;
        }

        public async Task SendNotifications(List<int> notificationIds)
        {
            var notifications = await _context.Notifications
                .Where(n => notificationIds.Contains(n.Id) && n.IsDeleted == false)
                    .Include(n => n.Users)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                foreach (var user in notification.Users)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(user.Email))
                            continue;

                        if (notification.Type == "expiration")
                        {
                            var membership = await _context.Memberships
                                .Where(m => m.Client.User.Id == user.Id && m.IsActive == true && m.IsDeleted == false)
                                .Include(m => m.Client)
                                    .ThenInclude(c => c.User)
                                .OrderByDescending(m => m.EndDate)
                                .FirstOrDefaultAsync();

                            if (membership == null) continue;

                            var today = DateOnly.FromDateTime(DateTime.UtcNow);
                            var daysRemaining = ((membership.EndDate.Year - today.Year) * 365)
                                + ((membership.EndDate.Month - today.Month) * 30)
                                + (membership.EndDate.Day - today.Day);

                            var fullName = $"{user.FirstName} {user.LastName}";

                            await _emailService.SendEmail(
                                user.Email,
                                "Subscription Expiration Notice",
                                fullName,
                                daysRemaining
                            );
                        }
                        else
                        {
                            // Handle other types of notifications here in future
                            // For now, skip or log
                            continue;
                        }

                        _logger.LogInformation("Sent notification {NotificationId} to {Email}", notification.Id, user.Email);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error sending notification {NotificationId} to {Email}", notification.Id, user.Email);
                    }
                }
            }
        }

        public async Task GenerateAndSendNotificationForUser(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.IsDeleted == true) 
                return;
            
            var membership = await _context.Memberships
                .Where(m => m.IsActive == true && m.IsDeleted == false && m.Client.User.Id == userId)
                .Include(m => m.Client)
                    .ThenInclude(c => c.User)
                .OrderByDescending(m => m.EndDate)
                .FirstOrDefaultAsync();


            if (membership == null || membership.IsAutorenewed == true) return;

            
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var daysRemaining = ((membership.EndDate.Year - today.Year)*365) +
                ((membership.EndDate.Month - today.Month)*30) +
                (membership.EndDate.Day - today.Day);
            
            var notification = new Notification
            {
                Message = $"Your subscription will expire in {daysRemaining} day(s).",
                Type = "expiration",
                Time = DateTime.UtcNow,
                IsGlobal = false,
                IsDeleted = false,
                Users = new List<User> { user }
            };

            var fullName = membership.Client.User.FirstName + " " + membership.Client.User.LastName;   
            await _emailService.SendEmail(user.Email, "Subscription Expiration Notice", fullName  ,daysRemaining);

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

    }
}
