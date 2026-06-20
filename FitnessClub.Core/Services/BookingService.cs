using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using FitnessClub_Test.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using FitnessClub_Test.Dtos;
using FitnessClub_Test.Core.NewModels;

public class BookingService : IBookingService
{
    private readonly FitnessClubDbContext _context;

    public BookingService(FitnessClubDbContext context)
    {
        _context = context;
    }

    public async Task<List<BookingViewDTO>> GetAllBookings()
    {
        return await _context.Bookings
            .Where(b => !b.IsDeleted)
            .Include(b => b.Class)
            .Include(b => b.Client)
                .ThenInclude(c => c.User)
            .Select(b => new BookingViewDTO
            {
                BookingID = b.Id,
                ClassName = b.Class.Name,
                ClientName = b.Client.User.FirstName + " " + b.Client.User.LastName,
                CreatedAt = b.Date ?? DateTime.UtcNow,
                Type = b.Type,
                Status = b.Status
            })
            .ToListAsync();
    }

    public async Task CreateBooking(BookingDTO dto)
    {
        try {        
            // 1. Validate class
            var fitnessClass = await _context.Classes
                .FirstOrDefaultAsync(c => c.Id == dto.ClassId && c.IsActive && !c.IsDeleted);

            if (fitnessClass == null)
                throw new Exception("Class not found or unavailable.");

            if (fitnessClass.MaxOccupancy == null || fitnessClass.MaxOccupancy <= 0)
                throw new Exception("Class capacity is not configured.");

            // 2. Check if client already booked this class
            bool alreadyBooked = await _context.Bookings
                .AnyAsync(b => b.ClassId == dto.ClassId && b.ClientId == dto.ClientId);

            if (alreadyBooked)
                throw new Exception("You have already booked this class.");

            // 3. Check for time conflicts
            var overlappingClass = await _context.Bookings
                .Include(b => b.Class)
                .Where(b => b.ClientId == dto.ClientId)
                .AnyAsync(b =>
                    b.Class.StartDate < fitnessClass.EndTime &&
                    fitnessClass.StartDate < b.Class.EndTime);

            if (overlappingClass)
                throw new Exception("You have a time conflict with another class.");

            // 4. Check occupancy
            var existingBookings = await _context.Bookings
                .CountAsync(b => b.ClassId == dto.ClassId);

            if (existingBookings >= fitnessClass.MaxOccupancy)
                throw new Exception("This class is already full.");

            // 5. Validate client 
            var clientExists = await _context.Clients.AnyAsync(c => c.Id == dto.ClientId);
            if (!clientExists)
                throw new Exception("Client does not exist.");

            // 6. Create booking
            var booking = new Booking
            {
                ClassId = dto.ClassId,
                ClientId = dto.ClientId,
                Date = DateTime.UtcNow,
                Status = "Confirmed",
                Type = dto.Type
            };

            _context.Bookings.Add(booking);

            await _context.SaveChangesAsync();
        }   
        catch (Exception ex)
        {
                Console.WriteLine("Error saving booking: " + ex.InnerException?.Message ?? ex.Message);
                throw;
        }
    }

    public async Task<BookingViewDTO?> GetBookingById(int id)
    {
        return await _context.Bookings
            .Where(b => b.Id == id && !b.IsDeleted)
            .Include(b => b.Class)
            .Include(b => b.Client)
                .ThenInclude(c => c.User)
            .Select(b => new BookingViewDTO
            {
                BookingID = b.Id,
                ClassID = b.ClassId,   // IMPORTANT for dropdown preselect
                ClientID = b.ClientId,
                ClassName = b.Class.Name,
                ClientName = b.Client.User.FirstName + " " + b.Client.User.LastName,
                CreatedAt = b.Date ?? DateTime.UtcNow,
                Type = b.Type,
                Status = b.Status
            })
            .FirstOrDefaultAsync();
    }

    public async Task<bool> UpdateBooking(int id, BookingDTO dto)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);

        if (booking == null)
            return false;

        booking.Type = dto.Type;
        booking.Status = dto.Status;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteBookingAsync(int bookingId)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == bookingId);

        if (booking == null)
            return false;

        booking.IsDeleted = true;
        await _context.SaveChangesAsync();

        return true;
    }
    
    public async Task<List<EnrolledClientDTO>> GetClientsByClass(int classId)
    {
        return await _context.Bookings
            .Where(cb => cb.ClassId == classId)
            .Select(cb => new EnrolledClientDTO
            {
                ClientID = cb.Client.Id,
                FullName = cb.Client.User.FirstName + " " + cb.Client.User.LastName,
                Email = cb.Client.User.Email
            })
            .ToListAsync();
    }

}
