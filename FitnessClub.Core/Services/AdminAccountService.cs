using FitnessClub_Test.Core.Interfaces;
using FitnessClub_Test.Core.NewModels;
using FitnessClub_Test.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace FitnessClub_Test.Core.Services
{
    public class AdminAccountService : IAdminAccountService
    {
        private readonly FitnessClubDbContext _context;

        public AdminAccountService(FitnessClubDbContext context)
        {
            _context = context;
        }
        public async Task<AdminAccountDTO?> GetAdminByIdAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return null;

            return new AdminAccountDTO
            {
                First_Name = user.FirstName,
                Last_Name = user.LastName,
                Email = user.Email,
                Phone_Number = user.PhoneNumber,
                Address = user.Address,
                DOB = user.Dob,
                Photo = user.Photo,
                DateCreated = user.DateCreated,
                Gender = user.Gender,
                QrCode = user.QrCode
            };
        }
        public async Task<(bool Success, string Error)> UpdateAsync(int userId, AdminAccountDTO dto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return (false, "User not found");

            // Email uniqueness check
            if (!string.IsNullOrWhiteSpace(dto.Email) &&
                !string.Equals(user.Email, dto.Email, StringComparison.OrdinalIgnoreCase))
            {
                var emailExists = await _context.Users.AnyAsync(u =>
                    u.Email == dto.Email && u.Id != userId);

                if (emailExists)
                    return (false, "Email already exists");
            }

            user.FirstName = dto.First_Name;
            user.LastName = dto.Last_Name;
            user.Email = dto.Email;
            if (!string.IsNullOrWhiteSpace(dto.Photo))
            {
                user.Photo = dto.Photo;
            }

            user.Address = dto.Address;
            if (dto.DOB.HasValue)
            {
                user.Dob = dto.DOB.Value;
            }
            user.PhoneNumber = dto.Phone_Number;
            user.Gender = string.IsNullOrEmpty(dto.Gender) ? null : dto.Gender.Substring(0, 1);
            user.QrCode = dto.QrCode;

            await _context.SaveChangesAsync();
            return (true, null);
        }
    }
}
