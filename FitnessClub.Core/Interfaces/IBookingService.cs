using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using FitnessClub_Test.Dtos;

public interface IBookingService
{
    Task CreateBooking(BookingDTO dto);
    Task<List<EnrolledClientDTO>> GetClientsByClass(int classId);
    Task<List<BookingViewDTO>> GetAllBookings();

    Task<BookingViewDTO?> GetBookingById(int id);
    Task<bool> UpdateBooking(int id, BookingDTO dto);

    Task<bool> DeleteBookingAsync(int bookingId);
}
