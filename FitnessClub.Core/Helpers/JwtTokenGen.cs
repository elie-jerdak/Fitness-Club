using FitnessClub_Test.Core.NewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FitnessClub_Test.Api
{
    public class JwtTokenGen
    {
        private readonly IConfiguration _config;
        private readonly UserManager<User> _userManager;

        /*Injects IConfiguration to access values from appsettings.json (like secret key, issuer, audience, etc.)*/
        public JwtTokenGen(IConfiguration config, UserManager<User> userManager)
        {
            _config = config;
            _userManager = userManager;
        }

        public async Task<string> GenerateToken(User user)
        {
            var claims = new List<Claim>
{
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),   // UserId (PRIMARY ID)
            new Claim(JwtRegisteredClaimNames.Email, user.Email),

            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),     // ASP.NET Core Identity standard
            new Claim(ClaimTypes.Name, user.Email),                  

            new Claim("FirstName", user.FirstName ?? ""),
            new Claim("LastName", user.LastName ?? ""),
            new Claim("QrCode", user.QrCode ?? "")
        };

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles) {
                claims.Add(new Claim(ClaimTypes.Role, role)); // Admin / Client / Coach
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_config.GetValue<int>("JwtSettings:ExpiresInMinutes")),
                signingCredentials: creds
             );


            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}
