using FitnessClub_Test.Core.NewModels;
using FitnessClub_Test.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessClub_Test.Core.Interfaces
{
    public interface INotificationService
    {
        Task<List<Notification>> GenerateExpirationNotifications();
        Task SendNotifications(List<int> notificationIds);
        Task GenerateAndSendNotificationForUser(int userId);

    }
}
