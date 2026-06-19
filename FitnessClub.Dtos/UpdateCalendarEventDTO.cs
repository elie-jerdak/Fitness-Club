using System;

namespace FitnessClub_Test.Dtos
{
    public class UpdateCalendarEventDTO
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public bool IsBackground { get; set; } = false;
        public string? Color { get; set; }
        public string? BackgroundColor { get; set; }

        public string Type { get; set; } = "Group";        // Group, Private, Admin, Special
        public string Visibility { get; set; } = "Public"; // Public, Members, Coach, Admin


        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public DateTime StartTime { get => Start; set => Start = value; }
        public DateTime EndTime { get => End; set => End = value; }

        public bool IsAllDay { get; set; } = false;
        public bool IsRecurring { get; set; } = false;

        // Weekly recurrence (comma-separated, e.g., "MO,TU")
        public string? DaysOfWeek { get; set; }

        // RRule format for FullCalendar
        public string? RRule { get; set; }

        public DateTime? EndRecur { get; set; }

        // Exception dates (comma-separated)
        public string? ExDates { get; set; }

        public int? CoachID { get; set; }
        public int? ClientID { get; set; }

        // Optional group ID for recurring series
        public string? GroupId { get; set; }

        // Optional FullCalendar display property
        public string? Display { get; set; } // "auto" or "background"
    }
}