using FitnessClub_Test.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessClub_Test.Core.Interfaces
{
    public interface ICalendarService
    {
        Task<List<FullCalendarEventDTO>> GetCalendarEvents(string role, int userID, DateTime? start = null, DateTime? end = null);
        Task<bool> CreateEvent(CreateCalendarEventDTO dto);
        Task<bool> UpdateEvent(UpdateCalendarEventDTO dto);
        Task<bool> DeleteEvent(int eventId, bool deleteSeries = false);
    }
}
