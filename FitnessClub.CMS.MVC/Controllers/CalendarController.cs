using FitnessClub_Test.CMS.MVC.Services;
using FitnessClub_Test.Core.Helpers;
using FitnessClub_Test.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace FitnessClub_Test.CMS.MVC.Controllers
{
    public class CalendarController : BaseController
    {
        private readonly ApiAuthClient _api;
        public CalendarController(ApiAuthClient api) : base(api)
        {
            _api = api;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Events(string start, string end)
        {
            var client = await _api.GetAuthorizedClientAsync();

            var token = HttpContext.Session.GetString("access");
            JwtHelper.GetUserID(token, out int userID);

            var encodedStart = Uri.EscapeDataString(start);
            var encodedEnd = Uri.EscapeDataString(end);

            // call the API with the start/end from FullCalendar
            var response = await client.GetAsync($"calendar/events?start={encodedStart}&end={encodedEnd}&userID={userID}");

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            var events = JsonSerializer.Deserialize<object>(json);

            return Json(events);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCalendarEventDTO dto)
        {
            var client = await _api.GetAuthorizedClientAsync();

            var response = await client.PostAsJsonAsync("calendar/create", dto);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateCalendarEventDTO dto)
        {
            var client = await _api.GetAuthorizedClientAsync();

            var response = await client.PutAsJsonAsync($"calendar/update-events/{dto.Id}", dto);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id,bool applySeries = false)
        {
            var client = await _api.GetAuthorizedClientAsync();

            var response = await client.DeleteAsync($"calendar/events/{id}?applySeries={applySeries}");
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            return Ok(result);
        }
    }
}
