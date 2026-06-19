using FitnessClub_Test.Core.Interfaces;
using FitnessClub_Test.Core.NewModels;
using FitnessClub_Test.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessClub_Test.Core.Services
{
    public class MemberService : IMemberService
    {
        private readonly FitnessClubDbContext _context;
        private readonly IFileUploadService _fileUploadService;
        private readonly UserManager<User> _userManager;
        public MemberService(FitnessClubDbContext context, IFileUploadService fileUploadService, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
            _fileUploadService = fileUploadService;
        }

        public async Task<List<MemberDTO>> GetAllMembersAsync()
        {
            var members = await _context.Memberships
           .Where(m => m.IsActive && !m.IsDeleted &&
                       m.Client.User.IsActive && !m.Client.User.IsDeleted)
           .Include(m => m.Client)
               .ThenInclude(c => c.User)
           .Select(m => new MemberDTO
           {
               UserID = m.Client.User.Id,
               MemberID = m.Id,
               ClientID = m.ClientId,
               First_Name = m.Client.User.FirstName,
               Last_Name = m.Client.User.LastName,
               StartDate = m.StartDate,
               EndDate = m.EndDate,
               Photo = m.Client.User.Photo,
               Type = m.Type
           })
           .ToListAsync();

            return members;
        }

        public async Task<MemberEditDTO> GetMemberForUpdateAsync(int memberId)
        {
            var member = await _context.Memberships
                .Where(m => m.IsActive && !m.IsDeleted)
                .Include(m => m.Client)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == memberId);

            if (member == null) return null;

            return new MemberEditDTO
            {
                UserId = member.Client.User.Id,
                FirstName = member.Client.User.FirstName,
                LastName = member.Client.User.LastName,
                Address = member.Client.User.Address,
                Email = member.Client.User.Email,
                DOB = member.Client.User.Dob,
                Gender = member.Client.User.Gender,
                PhoneNumber = member.Client.User.PhoneNumber,
                QrCode = member.Client.User.QrCode,
                Photo = member.Client.User.Photo,

                Height = member.Client.Height,
                Weight = member.Client.Height,
                Target = member.Client.Target,
                MedicalHistory = member.Client.MedicalHistory,

                MemberID = member.Id,
                Type = member.Type
            };
        }

        public async Task<(bool Success, Dictionary<string, string[]> Errors)> UpdateMemberAsync(int memberId, MemberEditDTO dto)
        {
            var errors = new Dictionary<string, string[]>();

            var member = await _context.Memberships
                .Where(m => m.IsActive && !m.IsDeleted)
                .Include(m => m.Client)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == memberId);

            if (member == null)
            {
                errors.Add("_", new[] { "Member not found." });
                return (false, errors);
            }

            // Example forbidden check: only allow editing self (customize as needed)
            // int currentUserId = ...;
            // if (member.Client.UserId != currentUserId)
            // {
            //     errors.Add("_", new[] { "You do not have permission to edit this member." });
            //     return (false, errors);
            // }

            // Check email uniqueness
            var existingUser = await _context.Users
                .Where(u => u.Email == dto.Email && u.Id != member.Client.UserId)
                .FirstOrDefaultAsync();
            if (existingUser != null)
            {
                errors.Add("Email", new[] { "Email already in use." });
                return (false, errors);
            }

            // Update membership
            member.Type = dto.Type;
            member.IsAutorenewed = dto.IsAutoRenewed;

            // Update client
            member.Client.Height = dto.Height;
            member.Client.Weight = dto.Weight;
            member.Client.Target = dto.Target;
            member.Client.MedicalHistory = dto.MedicalHistory;

            // Update user
            var user = member.Client.User;
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Email = dto.Email;
            user.PhoneNumber = dto.PhoneNumber;
            user.Address = dto.Address;
            user.Dob = dto.DOB;
            if (!string.IsNullOrWhiteSpace(dto.Photo))
            {
                user.Photo = dto.Photo;
            }
            user.Gender = dto.Gender;
            user.QrCode = dto.QrCode;

            await _context.SaveChangesAsync();

            return (true, null);
        }

        public async Task<MemberDTO> GetMemberForDeleteAsync(int memberId)
        {
            var member = await _context.Memberships
            .Where(m => m.IsActive && !m.IsDeleted)
            .Include(m => m.Client)
                .ThenInclude(c => c.User)
            .FirstOrDefaultAsync(m => m.Id == memberId);

            if (member == null)
                return null;

            return new MemberDTO
            {
                UserID = member.Client.User.Id,
                MemberID = member.Id,
                Type = member.Type,
                StartDate = member.StartDate,
                EndDate = member.EndDate,
                First_Name = member.Client.User.FirstName,
                Last_Name = member.Client.User.LastName
            };
        }

        public async Task<bool> DeleteMemberAsync(int MemberID)
        {
            var member = await _context.Memberships
                .Where(m => m.IsActive && !m.IsDeleted)
                .Include(m => m.Client)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == MemberID);

            if (member == null)
                return false;

            // Soft delete membership
            member.IsDeleted = true;
            member.IsActive = false;

            // Soft delete user
            if (member.Client?.User != null)
            {
                member.Client.User.IsDeleted = true;
                member.Client.User.IsActive = false;
            }

            await _context.SaveChangesAsync();
            return true;
        }
    
        public async Task<BulkDeleteDTO> BulkDeleteMemberAsync(List<int> selectedMemberIDs)
        {
            var result = new BulkDeleteDTO();

            foreach (var id in selectedMemberIDs)
            {
                var success = await DeleteMemberAsync(id);
                if (success)
                    result.DeletedCount++;
                else
                    result.FailedUserIds.Add(id);
            }

            result.FailedCount = result.FailedUserIds.Count;
            return result;
        }
    
        public async Task<CreateFullMemberResultDTO> CreateMemberAsync(FullMemberDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Create User
                var user = new User
                {
                    FirstName = dto.user.First_Name,
                    LastName = dto.user.Last_Name,
                    Email = dto.user.Email,
                    Address = dto.user.Address,
                    Dob = dto.user.DOB,
                    QrCode = dto.user.QrCode,
                    Photo = dto.user.Photo,
                    PhoneNumber = dto.user.Phone_Number,
                    DateCreated = DateTime.Now,
                    Gender = dto.user.Gender,
                    UserName = dto.user.Email,
                    IsActive = true,
                    IsDeleted = false
                };

                // Check email uniqueness
                var existingUser = await _context.Users
                    .Where(u => u.Email == dto.user.Email)
                    .FirstOrDefaultAsync();

                if (existingUser != null)
                {
                    return new CreateFullMemberResultDTO
                    {
                        Success = false,
                        Errors = new List<string> { "Email already in use." },
                        Message = "Email already in use"
                    };
                }

                var result = await _userManager.CreateAsync(user, dto.user.Password);

                if (!result.Succeeded)
                {
                    return new CreateFullMemberResultDTO
                    {
                        Success = false,
                        Errors = result.Errors.Select(e => e.Description).ToList(),
                         Message = "Failed to create user"
                    };
                }

                if (!string.IsNullOrEmpty(dto.user.Role))
                    await _userManager.AddToRoleAsync(user, dto.user.Role);

                await _context.SaveChangesAsync(); // Get UserId

                int userId = user.Id;

                // 3. Create Client
                var client = new Client
                {
                    Height = dto.client.Height,
                    Weight = dto.client.Weight,
                    Target = dto.client.Target,
                    MedicalHistory = dto.client.MedicalHistory,
                    UserId = userId
                };

                _context.Clients.Add(client);
                await _context.SaveChangesAsync(); // Get ClientId
                int clientId = client.Id;

                var startDate = DateTime.UtcNow;

                DateTime endDate = dto.membership.Type switch
                {
                    "Trial" => startDate.AddDays(7),
                    "Monthly" => startDate.AddMonths(1),
                    "Yearly" => startDate.AddYears(1),
                    _ => throw new ArgumentException("Invalid membership type")
                };

                // 4. Create Member
                var membership = new Membership
                {
                    StartDate = DateOnly.FromDateTime(startDate),
                    EndDate = DateOnly.FromDateTime(endDate),
                    Type = dto.membership.Type,
                    ClientId = clientId,
                    IsDeleted = false,
                    IsActive = true,
                    IsAutorenewed = dto.membership.IsAutoRenewed
                };

                _context.Memberships.Add(membership);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return new CreateFullMemberResultDTO
                {
                    Success = true,
                    Message = "Member created successfully",
                    UserId = user.Id,
                    ClientId = client.Id,
                    MembershipId = membership.Id
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

        public async Task<MemberEditDTO> GetMemberByUserIdAsync(int userId)
        {
            var member = await _context.Memberships
                .Where(m => m.Client.UserId == userId && m.IsActive && !m.IsDeleted)
                .Include(m => m.Client)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync();

            if (member == null) return null;

            return new MemberEditDTO
            {
                UserId = member.Client.User.Id,
                FirstName = member.Client.User.FirstName,
                LastName = member.Client.User.LastName,
                Email = member.Client.User.Email,
                PhoneNumber = member.Client.User.PhoneNumber,
                Gender = member.Client.User.Gender,
                Address = member.Client.User.Address,
                DOB = member.Client.User.Dob,
                Height = member.Client.Height,
                Weight = member.Client.Weight,
                Target = member.Client.Target,
                MedicalHistory = member.Client.MedicalHistory,
                MemberID = member.Id,
                Type = member.Type,
                IsAutoRenewed = member.IsAutorenewed,
                Photo = member.Client.User.Photo,
                QrCode = member.Client.User.QrCode
            };
        }

        public async Task<List<PremadeProgramsDTO>> GetPurchasedProgramsAsync(int userId)
        {
            return await _context.Memberships
                .Where(m => m.Client.UserId == userId && m.IsActive && !m.IsDeleted)
                .SelectMany(m => m.Client.PremadePrograms)
                .Distinct()
                .Select(p => new PremadeProgramsDTO
                {
                    ID = p.Id,
                    Title = p.Title,
                    Category = p.Category,
                    Price = p.Price
                })
                .ToListAsync();
        }

        public async Task<List<AvailabilitiesBookedDTO>> GetAvailabilitiesBooked(int userId)
        {
            // Load all confirmed bookings for this client, including availability and coach info
            var bookings = await _context.AvailabilityBookings
                .Where(b => b.Client.UserId == userId && b.Status == "Confirmed" && b.IsDeleted == false)
                .Include(b => b.Availability)
                    .ThenInclude(a => a.Coach)
                        .ThenInclude(c => c.User)
                .ToListAsync();

            // Map to simplified DTO
            var result = bookings.Select(b => new AvailabilitiesBookedDTO
            {
                Day = b.Availability.Day,
                StartTime = b.Availability.StartTime,
                EndTime = b.Availability.EndTime,
                CoachName = $"{b.Availability.Coach.User.FirstName} {b.Availability.Coach.User.LastName}"
            }).ToList();

            return result;
        }
    }
    
}
