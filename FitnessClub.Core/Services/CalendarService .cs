using FitnessClub_Test.Core.Interfaces;
using FitnessClub_Test.Core.NewModels;
using FitnessClub_Test.Dtos;
using FitnessClub_Test.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessClub_Test.Core.Services
{
    public class CalendarService : ICalendarService
    {
        private readonly FitnessClubDbContext _context;

        public CalendarService(FitnessClubDbContext context)
        {
            _context = context;
        }

        public async Task<List<FullCalendarEventDTO>> GetCalendarEvents(string role, int userID, DateTime? start = null, DateTime? end = null)
        {
            var query = _context.Calendars.AsQueryable();
            query = query.Where(e => !e.IsDeleted);

            // Role-based filtering
            if (role == "Client")
            {
                query = query.Where(e => e.Visibility == "Public" || e.ClientID == userID);
            }
            else if (role == "Coach")
            {
                query = query.Where(e => e.Visibility == "Public" || e.CoachID == userID);
            }

            // Date range filter
            if (start.HasValue && end.HasValue)
            {
                query = query.Where(e => !e.IsRecurring ||
                    (e.StartTime <= end.Value && (e.EndRecur ?? e.StartTime.AddMonths(6)) >= start.Value));
            }

            var events = await query.ToListAsync();
            var result = new List<FullCalendarEventDTO>();

            foreach (var ev in events)
            {
                if (!ev.IsRecurring && ev.EndTime <= ev.StartTime)
                    continue;

                var dto = new FullCalendarEventDTO
                {
                    Id = ev.ID,
                    Title = ev.EventTitle,
                    Start = ev.IsAllDay ? ev.StartTime.ToLocalIso(false) : ev.StartTime.ToLocalIso(),
                    End = ev.IsAllDay ? ev.EndTime.AddDays(1).ToLocalIso(false) : ev.EndTime.ToLocalIso(),
                    AllDay = ev.IsAllDay,
                    Color = ev.Color,
                    Display = ev.Display,
                    IsBackground = ev.IsBackground,
                    BackgroundColor = ev.BackgroundColor,
                    GroupId = ev.GroupId,

                    ExtendedProps = new
                    {
                        Type = ev.Type,
                        Visibility = ev.Visibility,
                        Description = ev.Description,
                        CoachID = ev.CoachID,
                        ClientID = ev.ClientID,
                        RRule = ev.RecurrenceRule,
                        DaysOfWeek = ev.DaysOfWeek,
                        EndRecur = ev.EndRecur?.ToString("yyyy-MM-dd"),
                        ExDates = ev.ExDates
                    }
                };

                // ---------------- SINGLE EVENTS ----------------
                if (!ev.IsRecurring)
                {
                    if (ev.IsAllDay)
                    {
                        dto.Start = ev.StartTime.ToLocalIso(false);
                        dto.End = ev.EndTime.AddDays(1).ToLocalIso(false);
                    }
                    else
                    {
                        dto.Start = ev.StartTime.ToLocalIso();
                        dto.End = ev.EndTime.ToLocalIso();
                    }

                    result.Add(dto);
                    continue;
                }

                // ---------------- RRULE RECURRENCE ----------------
                if (!string.IsNullOrEmpty(ev.RecurrenceRule))
                {
                    dto.Rrule = ev.RecurrenceRule; // DO NOT modify stored rule

                    dto.Dtstart = ev.StartTime
                        .ToUniversalTime()
                        .ToString("yyyy-MM-ddTHH:mm:ssZ");

                    var durationMinutes = (ev.EndTime - ev.StartTime).TotalMinutes;
                    dto.Duration = durationMinutes > 0 ? $"PT{(int)durationMinutes}M" : "PT30M";

                    if (ev.IsBackground)
                    {
                        var firstOccurrenceLocal = ev.StartTime.ToLocalTime();

                        dto.Start = firstOccurrenceLocal.ToString("yyyy-MM-ddTHH:mm:ss");
                        dto.End = ev.EndTime.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss");

                        dto.GroupId ??= Guid.NewGuid().ToString();
                    }
                }

                // ---------------- SIMPLE WEEKLY RECURRENCE ----------------
                else if (!string.IsNullOrEmpty(ev.DaysOfWeek))
                {
                    dto.DaysOfWeek = ev.DaysOfWeek
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(d => DayMap.TryGetValue(d.Trim().ToUpper(), out var day) ? day.ToString() : null)
                        .Where(x => x != null)
                        .ToList();

                    dto.StartTime = ev.StartTime.ToLocalIso(true).Substring(11, 8);
                    dto.EndTime = ev.EndTime.ToLocalIso(true).Substring(11, 8);

                    dto.EndRecur = (ev.EndRecur ?? ev.StartTime.AddMonths(3)).ToLocalIso(false);
                }

                // ---------------- EXCEPTION DATES ----------------
                if (!string.IsNullOrEmpty(ev.ExDates))
                {
                    if (!string.IsNullOrEmpty(ev.DaysOfWeek) || !string.IsNullOrEmpty(ev.RecurrenceRule))
                    {
                        dto.Exdate = ev.ExDates
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(d =>
                            {
                                var dt = DateTime.Parse(d.Trim());

                                return new DateTime(
                                    dt.Year,
                                    dt.Month,
                                    dt.Day,
                                    ev.StartTime.Hour,
                                    ev.StartTime.Minute,
                                    ev.StartTime.Second
                                ).ToLocalIso();
                            })
                            .ToList();
                    }
                    else
                    {
                        dto.Exdate = ev.ExDates
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(d => DateTime.Parse(d.Trim()).ToLocalIso(false))
                            .ToList();
                    }
                }

                result.Add(dto);
            }

            return result;
        }

        public async Task<bool> CreateEvent(CreateCalendarEventDTO dto)
        {
            // --- Create Event Overlap Check ---
            var overlappingEvents = await _context.Calendars
                .Where(e => dto.Start < e.EndTime && dto.End > e.StartTime) // overlapping times
                .Where(e => e.Display == "background"       // blocked/background
                         || e.Type == dto.Type              // same type
                         || (dto.CoachID.HasValue && e.CoachID == dto.CoachID)) // coach conflict
                .ToListAsync();

            if (overlappingEvents.Any())
            {
                var titles = string.Join(", ", overlappingEvents.Select(e => e.EventTitle));
                throw new Exception($"This time slot conflicts with: {titles}");
            }

            var ev = new Calendar
            {
                EventTitle = dto.Title,
                Description = dto.Description,
                Type = dto.Type,
                CoachID = dto.CoachID,
                ClientID = dto.ClientID,
                Visibility = dto.Visibility,
                StartTime = dto.Start,
                EndTime = dto.End,
                IsRecurring = dto.IsRecurring,
                DaysOfWeek = dto.DaysOfWeek,
                Display = dto.IsBackground ? "background" : "auto",
                BackgroundColor = dto.BackgroundColor,
                Color = dto.BackgroundColor,
                RecurrenceRule = dto.RRule,
                IsBackground = dto.IsBackground,
                EndRecur = dto.EndRecur,
                ExDates = dto.ExDates,
                GroupId = dto.IsRecurring ? Guid.NewGuid().ToString() : null, // only recurring events get a group ID
                IsAllDay = dto.IsAllDay
            };

            Console.WriteLine($"//////////////////Client ID: {ev.ClientID}");
            _context.Calendars.Add(ev);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateEvent(UpdateCalendarEventDTO dto)
        {
            var ev = await _context.Calendars.FirstOrDefaultAsync(e => e.ID == dto.Id && !e.IsDeleted);
            if (ev == null) return false;

            // --- Update Event Overlap Check ---
            var overlappingEvents = await _context.Calendars
                .Where(e => !e.IsDeleted)                  // exclude deleted events
                .Where(e => e.ID != dto.Id)                // exclude current event
                .Where(e => dto.Start < e.EndTime && dto.End > e.StartTime) // overlapping times
                .Where(e => e.Display == "background"      // blocked/background
                         || e.Type == dto.Type             // same type
                         || (dto.CoachID.HasValue && e.CoachID == dto.CoachID)) // coach conflict
                .ToListAsync();

            if (overlappingEvents.Any())
            {
                var titles = string.Join(", ", overlappingEvents.Select(e => e.EventTitle));
                throw new Exception($"This time slot conflicts with: {titles}");
            }

            // Update whole recurring series if GroupId exists
            var series = string.IsNullOrEmpty(ev.GroupId)
                ? new List<Calendar> { ev }
                : await _context.Calendars.Where(e => e.GroupId == ev.GroupId).ToListAsync();

            foreach (var item in series)
            {
                item.EventTitle = dto.Title;
                item.Description = dto.Description;
                item.CoachID = dto.CoachID;
                item.ClientID = dto.ClientID;
                item.StartTime = dto.Start;
                item.EndTime = dto.End;
                item.IsAllDay = dto.IsAllDay;
                item.Color = dto.BackgroundColor;
                item.Display = dto.Display;
                item.Type = dto.Type;
                item.Visibility = dto.Visibility;
                item.DaysOfWeek = dto.DaysOfWeek;
                item.IsRecurring = dto.IsRecurring;
                item.BackgroundColor = dto.BackgroundColor;
                item.IsBackground = dto.IsBackground;
                item.RecurrenceRule = dto.RRule;
                item.EndRecur = dto.EndRecur;
                item.ExDates = dto.ExDates;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteEvent(int eventId, bool applySeries = false)
        {
            var ev = await _context.Calendars.FirstOrDefaultAsync(e => e.ID == eventId && !e.IsDeleted);
            if (ev == null)
                return false;

            // If recurring and deleteSeries = true, delete all events with the same GroupId
            if (applySeries && !string.IsNullOrEmpty(ev.GroupId))
            {
                var series = await _context.Calendars
                    .Where(e => e.GroupId == ev.GroupId && !e.IsDeleted)
                    .ToListAsync();

                foreach (var item in series)
                {
                    item.IsDeleted = true;
                }
            }
            else
            {
                ev.IsDeleted = true;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        private static string GetColorByType(string type)
        {
            return type switch
            {
                "Group" => "#3788d8",
                "Private" => "#3788d8",
                "Administrative" => "#28a745",
                "Special" => "#dc3545",
                _ => "black"
            };
        }
       
        private static readonly Dictionary<string, int> DayMap = new()
        {
            { "SU", 0 },
            { "MO", 1 },
            { "TU", 2 },
            { "WE", 3 },
            { "TH", 4 },
            { "FR", 5 },
            { "SA", 6 },
        };
    }
}
