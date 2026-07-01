using System;
using System.Collections.Generic;

namespace FitnessClub_Test.Core.NewModels;

public partial class Calendar
{
    public int ID { get; set; }
    public string EventTitle { get; set; }
    public string Description { get; set; }
    public bool IsDeleted { get; set; }
    public string Display { get; set; }
    // Event classification
    public string Type { get; set; }        // Group, Private, Administrative, Special
    public string Visibility { get; set; }  // public, private
    public bool IsBackground { get;set; }

    // Base event times
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public int? CoachID { get; set; }
    public int? ClientID { get; set; }

    // Recurrence support
    public bool IsRecurring { get; set; } = false;

    // Supports FullCalendar rrule
    public string? RecurrenceRule { get; set; }

    // For weekly recurring events
    public string? DaysOfWeek { get; set; } // "1,3,5"

    // End date of recurrence
    public DateTime? EndRecur { get; set; }

    // Exception dates
    public string? ExDates { get; set; } // "2026-03-12,2026-03-19"

    // For events that move together
    public string? GroupId { get; set; }

    public string? Color { get; set; }
    public string? BackgroundColor { get; set; }
    public bool IsAllDay { get; set; }

    // Navigation properties
    public virtual Coach Coach { get; set; }
    public virtual Client Client { get; set; }

    // Many-to-many attendees
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}