using System.Collections.Generic;

namespace FitnessClub_Test.Dtos
{
    public class RevenueChartDTO
    {
        public List<string> Categories { get; set; }
        public List<RevenueSeriesDto> Series { get; set; }
    }
}
