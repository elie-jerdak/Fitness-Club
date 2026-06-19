using FitnessClub_Test.Api;
using FitnessClub_Test.Core.NewModels;
using FitnessClub_Test.Core.Interfaces;
using FitnessClub_Test.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessClub_Test.Core.Services
{
    public class AutorizationService : IAutorizationService
    {
        private readonly UserManager<User> _userManager;
        private readonly JwtTokenGen _jwtTokenGen;

        public AutorizationService(UserManager<User> userManager, JwtTokenGen jwtTokenGen)
        {
            _userManager = userManager;
            _jwtTokenGen = jwtTokenGen;
        }

        public async Task<(bool Success, string Token, string RefreshToken)> LoginAsync(LoginDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) return (false, null, null);

            var valid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!valid) return (false, null, null);

            var token = await _jwtTokenGen.GenerateToken(user);

            var refresh = Guid.NewGuid().ToString();
            user.RefreshToken = refresh;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(14);

            await _userManager.UpdateAsync(user);
            return (true, token, refresh);
        }

        public async Task RevokeRefreshTokenAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;
            await _userManager.UpdateAsync(user);
        }

        public async Task<(bool Success, string Error)> RegisterAsync(RegisterDTO dto)
        {
            if (await _userManager.FindByEmailAsync(dto.Email) != null)
            {
                return (false, "This email is already in use.");
            }

            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Gender = dto.Gender,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                Dob = dto.Dob,
                DateCreated = dto.CreatedDateTime,
                Email = dto.Email,
                UserName = dto.Email
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                var error = string.Join(", ", result.Errors.Select(e => e.Description));
                return (false, error);
            }

            await _userManager.AddToRoleAsync(user, "Client"); // default role

            return (true, null);
        }

        public async Task<bool> PromoteToAdminAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;

            // Remove old role
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            // Assign Admin
            await _userManager.AddToRoleAsync(user, "Admin");

            return true;
        }

        public async Task<(bool Success, string Token, string Refresh)> RefreshAsync(string refreshToken)
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
                return (false, null, null);

            var jwt = await _jwtTokenGen.GenerateToken(user);

            // rotate refresh token
            user.RefreshToken = Guid.NewGuid().ToString();
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(14);
            await _userManager.UpdateAsync(user);

            return (true, jwt, user.RefreshToken);
        }

    }


}
