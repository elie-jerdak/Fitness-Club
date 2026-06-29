using FitnessClub_Test.Core.Interfaces;
using FitnessClub_Test.Dtos;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FitnessClub_Test.Core.Services
{
    public class FastApiService : IFastApiService
    {
        private readonly HttpClient _httpClient;

        public FastApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        } 

        public async Task<FastApiRatingDTO?> GetCoachRatingAsync(int coachId)
        {
            var response = await _httpClient.GetAsync($"/rate/{coachId}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<FastApiRatingDTO>();
            }
            return null;
        }
    }
}
