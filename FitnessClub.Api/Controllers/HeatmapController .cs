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
            SELECT '06:00:00'::time AS StartTime, '07:00:00'::time AS EndTime
            UNION ALL SELECT '07:00:00', '08:00:00'
            UNION ALL SELECT '08:00:00', '09:00:00'
            UNION ALL SELECT '09:00:00', '10:00:00'
            UNION ALL SELECT '10:00:00', '11:00:00'
            UNION ALL SELECT '11:00:00', '12:00:00'
            UNION ALL SELECT '12:00:00', '13:00:00'
            UNION ALL SELECT '13:00:00', '14:00:00'
            UNION ALL SELECT '14:00:00', '15:00:00'
            UNION ALL SELECT '15:00:00', '16:00:00'
            UNION ALL SELECT '16:00:00', '17:00:00'
            UNION ALL SELECT '17:00:00', '18:00:00'
            UNION ALL SELECT '18:00:00', '19:00:00'
            UNION ALL SELECT '19:00:00', '20:00:00'
            UNION ALL SELECT '20:00:00', '21:00:00'
            UNION ALL SELECT '21:00:00', '22:00:00'
        ),
        FilteredSessions AS (
            SELECT 
                s.""User_ID"",
                s.""TimeIn""::time AS StartTime,
                s.""TimeOut""::time AS EndTime,
                to_char(s.""TimeIn"", 'FMDay') AS WeekDay
            FROM ""CheckingInOut"" s
            INNER JOIN ""AspNetUsers"" u 
                ON u.""Id"" = s.""User_ID""
            LEFT JOIN ""AspNetUserRoles"" ur
                ON ur.""UserId"" = u.""Id""
            LEFT JOIN ""AspNetRoles"" r
                ON r.""Id"" = ur.""RoleId""
            
        ),
        CrossJoined AS (
            SELECT 
                ts.StartTime,
                ts.EndTime,
                fs.WeekDay,
                fs.""User_ID""
            FROM TimeSlots ts
            LEFT JOIN FilteredSessions fs
                ON fs.StartTime < ts.EndTime 
               AND fs.EndTime > ts.StartTime
        )
        SELECT 
            (to_char(StartTime, 'HH24:MI') || '-' || to_char(EndTime, 'HH24:MI')) AS ""TimeSlot"",
            COUNT(CASE WHEN WeekDay = 'Monday' THEN ""User_ID"" END) AS ""Mon"",
            COUNT(CASE WHEN WeekDay = 'Tuesday' THEN ""User_ID"" END) AS ""Tue"",
            COUNT(CASE WHEN WeekDay = 'Wednesday' THEN ""User_ID"" END) AS ""Wed"",
            COUNT(CASE WHEN WeekDay = 'Thursday' THEN ""User_ID"" END) AS ""Thu"",
            COUNT(CASE WHEN WeekDay = 'Friday' THEN ""User_ID"" END) AS ""Fri"",
            COUNT(CASE WHEN WeekDay = 'Saturday' THEN ""User_ID"" END) AS ""Sat"",
            COUNT(CASE WHEN WeekDay = 'Sunday' THEN ""User_ID"" END) AS ""Sun""
        FROM CrossJoined
        GROUP BY StartTime, EndTime
        ORDER BY StartTime;
    
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