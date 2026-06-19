using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessClub_Test.Dtos
{
    public class FullCalendarEventDTO
    {
        // Standard FullCalendar fields
        public int Id { get; set; }
        public string Title { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public string StartTime { get; set; }  
        public string EndTime { get; set; }    
        public bool AllDay { get; set; }
        public string Color { get; set; }
        public string BackgroundColor { get; set; }
        public string Display { get; set; } // e.g. "auto", "background", "inverse-background"
        public bool IsBackground { get; set; }
        public string GroupId { get; set; }

        // Recurrence fields
        public object Rrule { get; set; }
        public string Dtstart { get; set; }
        public string Duration { get; set; }
        public List<string> DaysOfWeek { get; set; }
        public string EndRecur { get; set; }
        public List<string> Exdate { get; set; }

        // All custom fields go here
        public object ExtendedProps { get; set; }
    }
}
