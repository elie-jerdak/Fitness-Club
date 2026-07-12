using FitnessClub_Test.Core.NewModels;
using FitnessClub_Test.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessClub_Test.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class HeatmapController : ControllerBase
    {
        private readonly FitnessClubDbContext _context;

        public HeatmapController(FitnessClubDbContext context)
        {
            _context = context;
        }

        [HttpGet("weekly")]
        public async Task<IActionResult> GetWeeklyHeatmap(
            [FromQuery] string Role,
            [FromQuery] string gender,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var sql = @"
WITH TimeSlots AS (
    SELECT '06:00:00'::time AS ""StartTime"", '07:00:00'::time AS ""EndTime""
    UNION ALL SELECT '07:00:00'::time, '08:00:00'::time
    UNION ALL SELECT '08:00:00'::time, '09:00:00'::time
    UNION ALL SELECT '09:00:00'::time, '10:00:00'::time
    UNION ALL SELECT '10:00:00'::time, '11:00:00'::time
    UNION ALL SELECT '11:00:00'::time, '12:00:00'::time
    UNION ALL SELECT '12:00:00'::time, '13:00:00'::time
    UNION ALL SELECT '13:00:00'::time, '14:00:00'::time
    UNION ALL SELECT '14:00:00'::time, '15:00:00'::time
    UNION ALL SELECT '15:00:00'::time, '16:00:00'::time
    UNION ALL SELECT '16:00:00'::time, '17:00:00'::time
    UNION ALL SELECT '17:00:00'::time, '18:00:00'::time
    UNION ALL SELECT '18:00:00'::time, '19:00:00'::time
    UNION ALL SELECT '19:00:00'::time, '20:00:00'::time
    UNION ALL SELECT '20:00:00'::time, '21:00:00'::time
    UNION ALL SELECT '21:00:00'::time, '22:00:00'::time
),

FilteredSessions AS (
    SELECT
        s.""User_ID"",
        s.""TimeIn""::time AS ""StartTime"",
        s.""TimeOut""::time AS ""EndTime"",
        TRIM(to_char(s.""TimeIn"", 'Day')) AS ""WeekDay""
    FROM public.""CheckingInOut"" s
    INNER JOIN public.""AspNetUsers"" u
        ON u.""Id"" = s.""User_ID""
    LEFT JOIN public.""AspNetUserRoles"" ur
        ON ur.""UserId"" = u.""Id""
    LEFT JOIN public.""AspNetRoles"" r
        ON r.""Id"" = ur.""RoleId""
    WHERE
        s.""TimeIn""::date >= @startDate
        AND s.""TimeOut""::date <= @endDate
        AND (@gender = '' OR LOWER(u.""Gender"") = LOWER(@gender))
        AND (@Role = '' OR r.""Name"" = @Role)
),

CrossJoined AS (
    SELECT
        ts.""StartTime"",
        ts.""EndTime"",
        fs.""WeekDay"",
        fs.""User_ID""
    FROM TimeSlots ts
    LEFT JOIN FilteredSessions fs
        ON fs.""StartTime"" < ts.""EndTime""
       AND fs.""EndTime"" > ts.""StartTime""
)

SELECT
    to_char(""StartTime"", 'HH24:MI') || '-' || to_char(""EndTime"", 'HH24:MI') AS ""TimeSlot"",
    COUNT(CASE WHEN ""WeekDay"" = 'Monday' THEN ""User_ID"" END) AS ""Mon"",
    COUNT(CASE WHEN ""WeekDay"" = 'Tuesday' THEN ""User_ID"" END) AS ""Tue"",
    COUNT(CASE WHEN ""WeekDay"" = 'Wednesday' THEN ""User_ID"" END) AS ""Wed"",
    COUNT(CASE WHEN ""WeekDay"" = 'Thursday' THEN ""User_ID"" END) AS ""Thu"",
    COUNT(CASE WHEN ""WeekDay"" = 'Friday' THEN ""User_ID"" END) AS ""Fri"",
    COUNT(CASE WHEN ""WeekDay"" = 'Saturday' THEN ""User_ID"" END) AS ""Sat"",
    COUNT(CASE WHEN ""WeekDay"" = 'Sunday' THEN ""User_ID"" END) AS ""Sun""
FROM CrossJoined
GROUP BY ""StartTime"", ""EndTime""
ORDER BY ""StartTime"";
";
            string genderCode = string.IsNullOrWhiteSpace(gender) ? "" : gender.Trim().Substring(0, 1).ToLower();
            var data = await _context
                .Set<TimeSlotHeatmapDto>()
                .FromSqlRaw(sql,
                    new Npgsql.NpgsqlParameter("@gender", genderCode),
                    new Npgsql.NpgsqlParameter("@Role", (object)Role ?? ""),
                    new Npgsql.NpgsqlParameter("@startDate", (object)startDate ?? DateTime.MinValue),
                    new Npgsql.NpgsqlParameter("@endDate", (object)endDate ?? DateTime.MaxValue)
                )
                .ToListAsync();

            // Format it for ApexCharts
            var apexData = data.Select(row => new
            {
                name = row.TimeSlot,
                data = new[] { row.Mon, row.Tue, row.Wed, row.Thu, row.Fri, row.Sat, row.Sun }
            });

            return Ok(apexData);
        }
    }
}