using FitnessClub_Test.Dtos;
using System.Threading.Tasks;


namespace FitnessClub_Test.Core.Interfaces
{
    public interface IAutorizationService
    {
        Task<(bool Success, string Error)> RegisterAsync(RegisterDTO dto);
        Task<(bool Success, string Token, string RefreshToken)> LoginAsync(LoginDTO dto);
        Task RevokeRefreshTokenAsync(int userId);
        Task<(bool Success, string Token, string Refresh)> RefreshAsync(string refreshToken);
        Task<bool> PromoteToAdminAsync(int userId);
    }

}