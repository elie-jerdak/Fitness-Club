using FitnessClub_Test.Core.NewModels;
using FitnessClub_Test.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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
                    SELECT CAST('06:00:00' AS TIME) AS StartTime, CAST('07:00:00' AS TIME) AS EndTime
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
                        s.User_ID,
                        CAST(s.TimeIn AS TIME) AS StartTime,
                        CAST(s.TimeOut AS TIME) AS EndTime,
                        DATENAME(WEEKDAY, s.TimeIn) AS WeekDay
                    FROM [FitnessClubDB_TEST].dbo.CheckingInOut s
                    INNER JOIN [FitnessClubDB_TEST].dbo.AspNetUsers u 
                        ON u.Id = s.User_ID
                    LEFT JOIN [FitnessClubDB_TEST].dbo.AspNetUserRoles ur
                        ON ur.UserId = u.Id
                    LEFT JOIN [FitnessClubDB_TEST].dbo.AspNetRoles r
                        ON r.Id = ur.RoleId
                    WHERE 
                        CAST(s.TimeIn AS DATE) >= @startDate
                        AND CAST(s.TimeOut AS DATE) <= @endDate
                        AND (@gender IS NULL OR @gender = '' OR LOWER(u.Gender) = LOWER(@gender))
                        AND (@Role IS NULL OR @Role = '' OR r.Name = @Role)
                ),
                CrossJoined AS (
                    SELECT 
                        FORMAT(DATEADD(HOUR, DATEPART(HOUR, ts.StartTime), '2000-01-01'), 'HH:mm') + '-' +
                        FORMAT(DATEADD(HOUR, DATEPART(HOUR, ts.EndTime), '2000-01-01'), 'HH:mm') AS TimeSlot,
                        fs.WeekDay,
                        fs.User_ID
                    FROM TimeSlots ts
                    LEFT JOIN FilteredSessions fs
                        ON fs.StartTime < ts.EndTime 
                       AND fs.EndTime > ts.StartTime
                )
                SELECT TimeSlot,
                    ISNULL([Monday], 0) AS Mon,
                    ISNULL([Tuesday], 0) AS Tue,
                    ISNULL([Wednesday], 0) AS Wed,
                    ISNULL([Thursday], 0) AS Thu,
                    ISNULL([Friday], 0) AS Fri,
                    ISNULL([Saturday], 0) AS Sat,
                    ISNULL([Sunday], 0) AS Sun
                FROM (
                    SELECT TimeSlot, WeekDay, User_ID
                    FROM CrossJoined
                ) AS SourceTable
                PIVOT (
                    COUNT(User_ID) FOR WeekDay IN (
                        [Monday], [Tuesday], [Wednesday], [Thursday],
                        [Friday], [Saturday], [Sunday]
                    )
                ) AS PivotTable
                ORDER BY TimeSlot;
            ";
            string genderCode = string.IsNullOrWhiteSpace(gender) ? "" : gender.Trim().Substring(0, 1).ToLower();
            var data = await _context
                .Set<TimeSlotHeatmapDto>()
                .FromSqlRaw(sql,
                    new SqlParameter("@gender", genderCode),
                    new SqlParameter("@Role", (object)Role ?? DBNull.Value),
                    new SqlParameter("@startDate", (object)startDate ?? DBNull.Value),
                    new SqlParameter("@endDate", (object)endDate ?? DBNull.Value)
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