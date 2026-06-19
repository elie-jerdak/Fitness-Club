using FitnessClub_Test.Core.Interfaces;
using FitnessClub_Test.Core.NewModels;
using FitnessClub_Test.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessClub_Test.Core.Services
{
    public class CoachService : ICoachService
    {
        private readonly FitnessClubDbContext _context;
        private readonly ILogger<CoachService> _logger;
        private readonly IFastApiService _fastApiService;
        private readonly ICalendarService _calendarService;
        private readonly IFileUploadService _fileUploadService;
        private readonly IEmailService _emailService;
        private readonly IPremadeProgramsService _premadeProgramsService;
        private readonly UserManager<User> _userManager;
        public CoachService(FitnessClubDbContext context, ILogger<CoachService> logger, IFastApiService fastApiService,
            IFileUploadService fileUploadService, IEmailService emailService, IPremadeProgramsService premadeProgramsService, 
            UserManager<User> userManager, ICalendarService calendarService)
        {
            _context = context;
            _logger = logger;
            _fastApiService = fastApiService;
            _fileUploadService = fileUploadService;
            _emailService = emailService;
            _premadeProgramsService = premadeProgramsService;
            _userManager = userManager;
            _calendarService = calendarService;
        }

        public async Task<List<CoachDTO>> GetCoachesAsync()
        {
            var coaches = await _context.Coaches
                .Include(c => c.User)
                .Where(m => m.User.IsActive == true && m.User.IsDeleted == false)
                .Select(m => new CoachDTO
                {
                    FullName = m.User.FirstName + " " + m.User.LastName,
                    Specialty = m.Specialty,
                    YearsExperience = m.Experience,
                    Salary = m.Salary,
                    Bio = m.Bio,
                    UserID = m.UserId
                }).ToListAsync();
            
            if (coaches == null || coaches.Count == 0)
                throw new KeyNotFoundException("No active coaches found.");

            return coaches;
        }

        public async Task<CoachEditDTO> EditCoach(int UserID)
        {
            var coachEntity = await _context.Coaches
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserId == UserID);

            if (coachEntity == null)
                return null;

            var roles = await _userManager.GetRolesAsync(coachEntity.User);
            var role = roles.FirstOrDefault();
            
            var coach = new CoachEditDTO
            {
                UserID = coachEntity.User.Id,
                First_Name = coachEntity.User.FirstName,
                Last_Name = coachEntity.User.LastName,
                Email = coachEntity.User.Email,
                Gender = coachEntity.User.Gender,
                Address = coachEntity.User.Address,
                Phone_Number = coachEntity.User.PhoneNumber,
                Photo = coachEntity.User.Photo,
                DOB = coachEntity.User.Dob,
                Role = role,
                QrCode = coachEntity.User.QrCode,

                Specialty = coachEntity.Specialty,
                YearsExperience = coachEntity.Experience,
                Salary = coachEntity.Salary,
                Bio = coachEntity.Bio
            };

            return coach;
        }

        public async Task<bool> UpdateCoachAsync(CoachEditDTO dto)
        {

            Console.WriteLine($"this is the dto user id inside the service controller: {dto.UserID}");

            var coach = await _context.Coaches
                    .Include(c => c.User)
                        .Where(c => c.User.IsActive == true && c.User.IsDeleted == false && c.UserId == dto.UserID)
                            .FirstOrDefaultAsync();

            if (coach == null)
            {
                Console.WriteLine("coach is not found");
                return false;
            }

            // Check email uniqueness (UPDATE)
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Email == dto.Email &&
                    u.Id != dto.UserID
                );

            if (existingUser != null)
            {
                return false;
            }


            coach.User.FirstName = dto.First_Name;
            coach.User.LastName = dto.Last_Name;
            coach.User.Email = dto.Email;
            coach.User.Address = dto.Address;
            coach.User.Dob = dto.DOB;
            coach.User.Gender = dto.Gender;
            coach.User.PhoneNumber = dto.Phone_Number;

            if (!string.IsNullOrWhiteSpace(dto.Photo))
            {
                coach.User.Photo = dto.Photo;
            }

            coach.User.QrCode = dto.QrCode;

            // Update coach fields
            coach.Experience = dto.YearsExperience;
            coach.Specialty = dto.Specialty;
            coach.Salary = dto.Salary;
            coach.Bio = dto.Bio;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<CoachDTO> DeleteCoachAsync(int UserID)
        {
            var coachEntity = await _context.Coaches
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserId == UserID);

            if (coachEntity == null)
                return null;

            var coach = new CoachDTO
            {
                UserID = coachEntity.User.Id,
                FullName = coachEntity.User.FirstName + " " + coachEntity.User.LastName,
                Specialty = coachEntity.Specialty,
                YearsExperience = coachEntity.Experience,
                Salary = coachEntity.Salary,
                Bio = coachEntity.Bio
            };

            return coach;
        }
        
        public async Task<bool> ConfirmDelete(int UserID)
        {
            var coach = await _context.Coaches
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.User.IsActive == true && c.User.IsDeleted == false && c.UserId == UserID);

            if (coach == null) return false;

            coach.User.IsDeleted = true;
            coach.User.IsActive = false;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<BulkDeleteDTO> BulkDelete(List<int> selectedCoachID)
        {
            var result = new BulkDeleteDTO();

            foreach (var id in selectedCoachID)
            {
                var success = await ConfirmDelete(id);
                if (success)
                    result.DeletedCount++;
                else
                    result.FailedUserIds.Add(id);
            }

            result.FailedCount = result.FailedUserIds.Count;
            return result;
        }
    
        public async Task<CreateFullMemberResultDTO> ConfirmCreate(CoachCreateDTO coachCreateDTO)
        {
            Console.WriteLine($"Inside confirmCreate of Coach Service");
            Console.WriteLine($"coachCreateDTO.Photo: {coachCreateDTO.Photo}");
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Create User
                var user = new User
                {
                    FirstName = coachCreateDTO.First_Name,
                    LastName = coachCreateDTO.Last_Name,
                    Email = coachCreateDTO.Email,
                    UserName = coachCreateDTO.Email,
                    Address = coachCreateDTO.Address,
                    Dob = coachCreateDTO.DOB,
                    QrCode = coachCreateDTO.QrCode,
                    Photo = coachCreateDTO.Photo,
                    PhoneNumber = coachCreateDTO.Phone_Number,
                    DateCreated = DateTime.Now,
                    Gender = coachCreateDTO.Gender,
                    IsActive = true,
                    IsDeleted = false
                };

                // Check email uniqueness (CREATE)
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == coachCreateDTO.Email);

                if (existingUser != null)
                {
                    return new CreateFullMemberResultDTO
                    {
                        Success = false,
                        Errors = new List<string> { "Email already in use." },
                        Message = "Email already in use"
                    };
                }

                var result = await _userManager.CreateAsync(user, coachCreateDTO.Password);

                if (!result.Succeeded)
                {
                    // I reused that DTO instead of creating a new one just to change it's name. maybe later make it have a generic name
                    return new CreateFullMemberResultDTO
                    {
                        Success = false,
                        Errors = result.Errors.Select(e => e.Description).ToList(),
                        Message = "Failed to create user"
                    };
                }

                if (!string.IsNullOrEmpty(coachCreateDTO.Role))
                    await _userManager.AddToRoleAsync(user, coachCreateDTO.Role);

                await _context.SaveChangesAsync(); // Get UserId

                int userId = user.Id;

                // 2. Create Coach
                var coach = new Coach
                {
                    Experience = coachCreateDTO.YearsExperience,
                    Salary = coachCreateDTO.Salary,
                    Specialty = coachCreateDTO.Specialty,
                    Bio = coachCreateDTO.Bio,
                    UserId = userId
                };

                _context.Coaches.Add(coach);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                var CoachID = coach.Id;
                return new CreateFullMemberResultDTO
                {
                    Success = true,
                    Message = "Member created successfully",
                    UserId = user.Id,
                    CoachID = coach.Id
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new CreateFullMemberResultDTO
                {
                    Success = false,
                    Message = ex.Message,
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<FullCoachDTO> GetCoachDetails(int userId)
        {
            if (userId == 0)
                return null;

            var coach = await _context.Coaches
                .Include(c => c.User)
                    .Include(c => c.Availabilities)
                        .ThenInclude(c => c.AvailabilityBookings)
                        .Where(c => c.UserId == userId)
                        .Select(c => new FullCoachDTO
                        {
                            user = new UserDTO
                            {
                                Id = c.User.Id,
                                First_Name = c.User.FirstName,
                                Last_Name = c.User.LastName,
                                Email = c.User.Email,
                                Phone_Number = c.User.PhoneNumber,
                                Address = c.User.Address,
                                DOB = c.User.Dob,
                                Photo = c.User.Photo,
                                Gender = c.User.Gender,
                                QrCode = c.User.QrCode
                            },
                            coach = new CoachDTO
                            {
                                CoachID = c.Id,
                                YearsExperience = c.Experience,
                                Salary = c.Salary,
                                Specialty = c.Specialty,
                                Bio = c.Bio,
                                UserID = c.UserId
                            },
                            availability = c.Availabilities
                                .Where(a => a.IsDeleted == false && a.IsActive == true)
                                .Select(a => new
                                {
                                    Availability = a,
                                    LatestBooking = a.AvailabilityBookings
                                        .Where(b => b.IsDeleted != true)  
                                        .OrderByDescending(b => b.Id)
                                        .FirstOrDefault()
                                })
                                .Select(x => new AvailabilityDTO
                                {
                                    Id = x.Availability.Id,
                            
                                    ClientID = x.LatestBooking != null
                                        ? x.LatestBooking.ClientId
                                        : null,
                            
                                    Day = x.Availability.Day,
                                    StartTime = (TimeOnly)x.Availability.StartTime,
                                    EndTime = (TimeOnly)x.Availability.EndTime,
                            
                                    Status = x.LatestBooking != null
                                        ? x.LatestBooking.Status
                                        : "Free",
                            
                                    ClientFullName = x.LatestBooking != null
                                        ? x.LatestBooking.Client.User.FirstName + " " +
                                          x.LatestBooking.Client.User.LastName
                                        : null
                                })
                                .ToList()
                        })
                        .FirstOrDefaultAsync();
            if (coach != null)
            {
                // Call your FastAPI to get the average rating + individual feedbacks
                var rating = await _fastApiService.GetCoachRatingAsync(coach.coach.CoachID);

                if (rating != null)
                {
                    coach.Rating = rating.average_stars;
                    coach.FeedbackCount = rating.feedback_count;

                    // Map individual reviews
                    coach.Reviews = rating.reviews.Select(r => new FeedbackDTO
                    {
                        FeedbackId = r.FeedbackId,
                        ClientName = r.ClientName,
                        ClientProfilePicture = r.ClientProfilePicture,
                        Stars = r.Stars,
                        Comment = r.Comment,
                        Date = r.Date
                    }).ToList();
                }

                var programs = await _premadeProgramsService.GetPremadeProgramsByCoachIdAsync(coach.coach.CoachID);
                coach.programs = programs;
            }

            return coach;
        }

        public async Task<bool> FreeTimeSlotAsync(int availabilityId, int clientId)
        {
            var client = await _context.Clients
                .Include(c => c.User)
                .FirstOrDefaultAsync(b => b.Id == clientId);

            var booking = await _context.AvailabilityBookings
                .FirstOrDefaultAsync(b => b.AvailabilityId == availabilityId && b.ClientId == clientId);

            if (booking == null)
                return false;

            booking.IsDeleted = true;
            booking.Status = "Cancelled";

            await _context.SaveChangesAsync();
            
            var EmailAddress = client.User.Email;
            var subject = "Reservation Cancellation";
            var Header = "Time Slot Reservation Notification !!";
            var FirstName = client.User.FirstName;
            var Message = "Your reservation has been cancelled";

            await _emailService.SendEmail(EmailAddress, subject, Header, FirstName, Message);
            return true;
        }

        public async Task<int> ReserveSlot(int availabilityId, int clientId)
        {
            // Load availability including existing bookings
            var availability = await _context.Availabilities
                .Include(a => a.Coach)
                .Include(a => a.AvailabilityBookings)
                .FirstOrDefaultAsync(a => a.Id == availabilityId);

            // Load client including user info
            var client = await _context.Clients
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == clientId);

            if (availability == null || client == null)
                return -1; // not found

            // Check if slot is already booked by this client
            var existingBooking = availability.AvailabilityBookings
                .FirstOrDefault(b => b.ClientId == clientId);

            if (existingBooking != null)
                return -2; // already booked by this client

            // prevent any booking if slot is already taken by another client
            if (availability.AvailabilityBookings.Any() && availability.AvailabilityBookings.FirstOrDefault().IsDeleted == false)
                return -3; // slot already reserved

            // Create new booking
            var booking = new AvailabilityBooking
            {
                AvailabilityId = availabilityId,
                ClientId = clientId,
                Status = "Confirmed",
                Date = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            _context.AvailabilityBookings.Add(booking);
            await _context.SaveChangesAsync();

            // Send confirmation email
            var EmailAddress = client.User.Email;
            var subject = "Time Slot Reservation";
            var Header = "Time Slot Reserved!!";
            var FirstName = client.User.FirstName;
            var Message = "Your reservation has been confirmed";

            await _emailService.SendEmail(EmailAddress, subject, Header, FirstName, Message);

            // Create calendar event
            var eventDto = new CreateCalendarEventDTO
            {
                Title = "Private Session",
                Description = "Private Session",

                Type = "Private",
                BackgroundColor = "red",

                Start = availability.Day.ToDateTime(availability.StartTime),
                End = availability.Day.ToDateTime(availability.EndTime),

                CoachID = availability.CoachId,
                ClientID = clientId,

                IsBackground = false,
                IsAllDay = false,
                IsRecurring = false
            };

            await _calendarService.CreateEvent(eventDto);


            return availability.Coach.UserId; // used by MVC to redirect
        }

        public async Task<int> CreateAvailability(CreateAvailabilityViewModel dto)
        {
            // Load coach
            var coach = await _context.Coaches
                .FirstOrDefaultAsync(c => c.UserId == dto.CoachUserId);

            if (coach == null)
                return -1;

            // Prevent overlapping times
            var overlap = await _context.Availabilities.AnyAsync(a =>
                a.CoachId == coach.Id &&
                a.IsDeleted != true &&
                a.Day == dto.Day &&
                dto.StartTime < a.EndTime &&
                dto.EndTime > a.StartTime
            );

            if (overlap)
                return -2;

            var availability = new Availability
            {
                CoachId = coach.Id,
                Day = dto.Day,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                IsActive = true,
                IsDeleted = false
            };

            _context.Availabilities.Add(availability);
            await _context.SaveChangesAsync();

            return dto.CoachUserId;
        }

        public async Task<bool> DeleteAvailabilityAsync(int availabilityId)
        {
            var availability = await _context.Availabilities
                .Include(a => a.AvailabilityBookings)
                .FirstOrDefaultAsync(a => a.Id == availabilityId);

            if (availability == null)
                return false; // not found

            availability.IsActive = false;
            availability.IsDeleted = true;

            //  notify booked client(s)
            var booking = availability.AvailabilityBookings.FirstOrDefault();
            if (booking != null)
            {
                // Example email logic
                var client = await _context.Clients
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.Id == booking.ClientId);

                if (client != null)
                {
                    await _emailService.SendEmail(
                        client.User.Email,
                        "Availability Cancelled",
                        "Your booked session was cancelled",
                        client.User.FirstName,
                        $"Your session on {availability.Day} from {availability.StartTime} to {availability.EndTime} was cancelled."
                    );
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<AvailabilityDTO?> GetAvailabilityById(int id)
        {
            var availability = await _context.Availabilities
                                    .FirstOrDefaultAsync(a => a.Id == id && a.IsDeleted == false && a.IsActive == true);

            if (availability == null)
                return null;

            // Check if there is a booking for this slot
            var booking = await _context.AvailabilityBookings
                .Include(b => b.Client)
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(b => b.AvailabilityId == id);

            var name = booking != null ? $"{booking.Client?.User.FirstName} {booking.Client?.User.LastName}" : null;

            return new AvailabilityDTO
            {
                Id = availability.Id,
                Day = availability.Day,
                StartTime = availability.StartTime,
                EndTime = availability.EndTime,
                CoachId = availability.CoachId,
                Status = booking != null ? "Booked" : "Free",
                ClientID = booking?.ClientId,
                ClientFullName = name
            };
        }

        public async Task<bool> UpdateAvailability(AvailabilityDTO dto)
        {
            var entity = await _context.Availabilities
                .Include(a => a.AvailabilityBookings)
                    .ThenInclude(b => b.Client)
                        .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(a => a.Id == dto.Id);

            if (entity == null)
                return false;

            // STEP 1: Check for overlapping availability for SAME coach
            bool hasOverlap = await _context.Availabilities
                .Where(a => a.CoachId == entity.CoachId && a.Id != dto.Id) // same coach, exclude current
                .AnyAsync(a =>
                    a.Day == dto.Day &&
                    dto.StartTime < a.EndTime &&
                    dto.EndTime > a.StartTime
                );

            if (hasOverlap)
                return false; 

            // STEP 2: Update fields
            entity.Day = dto.Day;
            entity.StartTime = dto.StartTime;
            entity.EndTime = dto.EndTime;

            await _context.SaveChangesAsync();

            // STEP 3: Notify if booked
            var booking = entity.AvailabilityBookings
                .FirstOrDefault(b => b.Status == "Confirmed");

            if (booking != null)
            {
                var client = booking.Client;
                var email = client.User.Email;
                var firstName = client.User.FirstName;

                var subject = "Availability Updated";
                var header = "Your booked time slot has been updated!";
                var message = $"Hello {firstName},\n\nThe availability you booked on {entity.Day:yyyy-MM-dd} from {entity.StartTime:HH:mm} to {entity.EndTime:HH:mm} has been updated. Please check your schedule.";

                await _emailService.SendEmail(email, subject, header, firstName, message);
            }

            return true;
        }

        public async Task<int> GetCoachUserIdByAvailabilityId(int availabilityId)
        {
            return await _context.Availabilities
                .Where(a => a.Id == availabilityId)
                .Select(a => a.Coach.UserId)
                .FirstOrDefaultAsync();
        }
    }
}
