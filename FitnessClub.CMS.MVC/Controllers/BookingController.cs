using FitnessClub_Test.CMS.MVC.Services;
using FitnessClub_Test.Core.NewModels;
using FitnessClub_Test.Dtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FitnessClub_Test.CMS.MVC.Controllers
{
    public class BookingController : BaseController
    {
        private readonly ApiAuthClient _client;
        private readonly FitnessClubDbContext _context;

        public BookingController(ApiAuthClient client, FitnessClubDbContext context) : base(client)
        {
            _client = client;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var client = await _client.GetAuthorizedClientAsync();

            var bookings = await client.GetFromJsonAsync<List<BookingViewDTO>>("booking/get-bookings");

            if (bookings == null)
                bookings = new List<BookingViewDTO>();

            return View("Index", bookings);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var client = await _client.GetAuthorizedClientAsync();

            var classes = await client.GetFromJsonAsync<List<ClassDTO>>("classes/get-classes");
            var members = await client.GetFromJsonAsync<List<MemberDTO>>("members/get-members");

            var clients = members?.Select(m => new EnrolledClientDTO
            {
                ClientID = m.ClientID, // make sure this exists
                FullName = $"{m.First_Name} {m.Last_Name}"
            }).ToList() ?? new List<EnrolledClientDTO>();

            var vm = new BookingCreateDTO
            {
                Classes = classes ?? new List<ClassDTO>(),
                Clients = clients
            };

            return View("CreateBooking", vm);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking(BookingCreateDTO model)
        {
            var client = await _client.GetAuthorizedClientAsync();

            // Map ViewModel → API DTO
            var bookingDto = new BookingDTO
            {
                ClassId = model.ClassID,
                ClientId = model.ClientID,
                Type = model.Type
            };

            // Call API endpoint: POST /api/booking/create
            var response = await client.PostAsJsonAsync("booking/create", bookingDto);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Booking failed";

                // VERY IMPORTANT: reload dropdowns
                var classes = await client.GetFromJsonAsync<List<ClassDTO>>("classes");
                var members = await client.GetFromJsonAsync<List<MemberDTO>>("members");

                model.Classes = classes ?? new List<ClassDTO>();

                model.Clients = members?.Select(m => new EnrolledClientDTO
                {
                    ClientID = m.ClientID,
                    FullName = $"{m.First_Name} {m.Last_Name}"
                }).ToList() ?? new List<EnrolledClientDTO>();

                return View("CreateBooking", model);
            }

            TempData["Success"] = "Booking created successfully";

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int bookingID)
        {
            var client = await _client.GetAuthorizedClientAsync();

            //  Get booking
            var booking = await client.GetFromJsonAsync<BookingViewDTO>($"booking/{bookingID}");

            if (booking == null)
            {
                TempData["Error"] = "Booking not found";
                return RedirectToAction("Index");
            }

            //  Get dropdown data
            var classes = await client.GetFromJsonAsync<List<ClassDTO>>("classes/get-classes");
            var members = await client.GetFromJsonAsync<List<MemberDTO>>("members/get-members");

            var clients = members?.Select(m => new EnrolledClientDTO
            {
                ClientID = m.ClientID,
                FullName = $"{m.First_Name} {m.Last_Name}"
            }).ToList() ?? new List<EnrolledClientDTO>();

            //  Map to Edit ViewModel
            var vm = new BookingCreateDTO
            {
                ClassID = booking.ClassID,     // you may need to include this in DTO
                ClientID = booking.ClientID,   // same here
                Type = booking.Type,
                Status = booking.Status,

                Classes = classes ?? new List<ClassDTO>(),
                Clients = clients
            };

            return View("EditBooking", vm);
        }

        [HttpPost]
        public async Task<IActionResult> EditBooking(BookingCreateDTO model)
        {
            var client = await _client.GetAuthorizedClientAsync();

            var bookingDto = new BookingDTO
            {
                ClassId = model.ClassID,
                ClientId = model.ClientID,
                Type = model.Type,
                Status = model.Status
            };

            var response = await client.PutAsJsonAsync(
                $"booking/update/{model.BookingID}", bookingDto);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Update failed";

                // reload dropdowns
                var classes = await client.GetFromJsonAsync<List<ClassDTO>>("classes/get-classes");
                var members = await client.GetFromJsonAsync<List<MemberDTO>>("members/get-members");

                model.Classes = classes ?? new List<ClassDTO>();
                model.Clients = members?.Select(m => new EnrolledClientDTO
                {
                    ClientID = m.ClientID,
                    FullName = $"{m.First_Name} {m.Last_Name}"
                }).ToList() ?? new List<EnrolledClientDTO>();

                return View("EditBooking", model);
            }

            TempData["Success"] = "Booking updated successfully";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int bookingId)
        {
            var client = await _client.GetAuthorizedClientAsync();

            var response = await client.DeleteAsync($"booking/{bookingId}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Failed to delete booking.";
                return RedirectToAction("Index");
            }

            TempData["Success"] = "Booking deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}