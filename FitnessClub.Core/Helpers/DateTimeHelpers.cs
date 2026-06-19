using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessClub_Test.Core.Helpers
{
    public static class DateTimeHelpers
    {
        /// <summary>
        /// Converts a UTC DateTime to local time and formats it for FullCalendar (ISO string)
        /// </summary>
        public static string ToLocalIso(this DateTime utcDateTime, bool includeTime = true)
        {
            var local = utcDateTime.ToLocalTime();

            if (includeTime)
                return local.ToString("yyyy-MM-ddTHH:mm:ss"); // local datetime for input fields
            else
                return local.ToString("yyyy-MM-dd"); // all-day events
        }
    }
}
