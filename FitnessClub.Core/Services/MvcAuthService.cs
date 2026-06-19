using FitnessClub_Test.Core.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace FitnessClub_Test.Core.Services
{
    public class MvcAuthService : IMvcAuthService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MvcAuthService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task SignInAsync(string jwtToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(jwtToken);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,
                jwt.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value),

            new Claim(ClaimTypes.Name,
                jwt.Claims.First(c => c.Type == ClaimTypes.Name).Value),

            new Claim(ClaimTypes.Email,
                jwt.Claims.First(c => c.Type == "email").Value),

            new Claim(ClaimTypes.Role,
                jwt.Claims.First(c => c.Type == ClaimTypes.Role).Value)
            };

            var identity = new ClaimsIdentity(
                claims,
                IdentityConstants.ApplicationScheme
            );

            var principal = new ClaimsPrincipal(identity);

            await _httpContextAccessor.HttpContext.SignInAsync(
                IdentityConstants.ApplicationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                }
            );
        }

        public async Task SignOutAsync()
        {
            await _httpContextAccessor.HttpContext!.SignOutAsync(
                IdentityConstants.ApplicationScheme
            );
        }
    }
}
