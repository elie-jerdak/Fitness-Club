using FitnessClub_Test.CMS.MVC.Services;
using FitnessClub_Test.Core.NewModels;
using FitnessClub_Test.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FitnessClub_Test.CMS.MVC.Controllers
{
    public class ClassController : BaseController
    {
        private readonly ApiAuthClient _client;
        private readonly FitnessClubDbContext _context;

        public ClassController(ApiAuthClient client, FitnessClubDbContext context) : base(client)
        {
            _client = client;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var client = await _client.GetAuthorizedClientAsync();

            var response = await client.GetAsync("classes/get-classes");

            if (!response.IsSuccessStatusCode)
            {
                // log or handle error
                var emptyList = new List<ClassDTO>();
                return View("Index", emptyList);
            }

            var json = await response.Content.ReadAsStringAsync();
            var classes = JsonConvert.DeserializeObject<List<ClassDTO>>(json)
                          ?? new List<ClassDTO>();

            return View("Index", classes);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View("CreateClass");
        }

        [HttpPost]
        public async Task<IActionResult> Create(ClassDTO dto)
        {
            var client = await _client.GetAuthorizedClientAsync();
            var response = await client.PostAsJsonAsync("classes/create-class", dto);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Failed to create class. Please try again.";
                return View(dto);
            }

            TempData["Success"] = "Class created successfully!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int ClassID)
        {
            var client = await _client.GetAuthorizedClientAsync();

            var response = await client.GetAsync($"classes/{ClassID}");

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var json = await response.Content.ReadAsStringAsync();
            var cls = JsonConvert.DeserializeObject<ClassDTO>(json);

            await PopulateDropdowns();

            return View("EditClass", cls);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ClassDTO dto)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View("EditClass", dto);
            }

            var client = await _client.GetAuthorizedClientAsync();

            var response = await client.PutAsJsonAsync($"classes/{dto.ClassID}", dto);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                TempData["Error"] = $"Failed to update class: {error}";

                await PopulateDropdowns();
                return View("EditClass", dto);
            }

            TempData["Success"] = "Class updated successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int ClassID)
        {
            var client = await _client.GetAuthorizedClientAsync();

            var response = await client.DeleteAsync($"classes/{ClassID}");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                TempData["Error"] = $"Failed to delete class: {error}";
                return RedirectToAction("Index");
            }

            TempData["Success"] = "Class deleted successfully!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult>Details(int classID)
        {
            var client = await _client.GetAuthorizedClientAsync();

            var classResponse = await client.GetAsync($"classes/{classID}");
            var clientsResponse = await client.GetAsync($"booking/{classID}/clients");
            var members = await client.GetFromJsonAsync<List<MemberDTO>>("members/get-members");

            if (!classResponse.IsSuccessStatusCode)
            {
                TempData["Error"] = "Class Not Found";
                return NotFound();
            }
                
            var classData = await classResponse.Content.ReadFromJsonAsync<ClassDTO>();

            List<EnrolledClientDTO> clients = new();

            if (clientsResponse.IsSuccessStatusCode)
            {
                clients = await clientsResponse.Content.ReadFromJsonAsync<List<EnrolledClientDTO>>();
            }

            var allClients = new List<EnrolledClientDTO>();

            if (members != null)
            {
                allClients = members.Select(m => new EnrolledClientDTO
                {
                    ClientID = m.ClientID,
                    FullName = m.First_Name + " " + m.Last_Name
                }).ToList();
            }

            var vm = new ClassDetailsDTO
            {
                Class = classData,
                Clients = clients,
                AllClients = allClients
            };

            await PopulateDropdowns();
            return View("ClassDetails",vm);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking(BookingDTO bookingDto)
        {
            var client = await _client.GetAuthorizedClientAsync();

            var response = await client.PostAsJsonAsync("booking/create", bookingDto);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Class Booking Failed";
            }
            else
            {
                TempData["Success"] = "Class Booked Successfully";
            }

            return RedirectToAction("Details", new { classID = bookingDto.ClassId });
        }

        private async Task PopulateDropdowns()
        {
            var coaches = await _context.Coaches
                .Include(c => c.User)
                .ToListAsync();

            ViewBag.Coaches = coaches;

            ViewBag.Recurrence = new List<string> {"None", "Daily", "Weekly", "Monthly"};

            ViewBag.Status = new List<string> { "Scheduled", "Cancelled" };
        }

    }
}