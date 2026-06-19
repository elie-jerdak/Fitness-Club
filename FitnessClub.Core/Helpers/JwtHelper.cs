using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FitnessClub_Test.Core.Helpers
{
    public class JwtHelper
    {
        public static string GetRole(string token)
        {
            if (string.IsNullOrEmpty(token)) 
                return null;

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            return jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        }

        public static bool GetUserID(string token, out int userId)
        {
            userId = default;

            if (string.IsNullOrWhiteSpace(token))
                return false;

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);

                var claim = jwt.Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

                if (claim == null)
                    return false;

                return int.TryParse(claim.Value, out userId);
            }
            catch
            {
                return false;
            }
        }

        public static string GetFirstName(string token)
        {
            if (string.IsNullOrEmpty(token))
                return null;

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            var firstName = jwt.Claims.First(c => c.Type == "FirstName");

            return firstName?.Value;
        }
        public static string GetLastName(string token)
        {
            if (string.IsNullOrEmpty(token))
                return null;

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            var lastName = jwt.Claims.First(c => c.Type == "LastName");

            return lastName?.Value;
        }
    }
}
