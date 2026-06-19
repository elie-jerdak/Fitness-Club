using System.Collections.Generic;

namespace FitnessClub_Test.Dtos
{
    public class FinanceTableDto
    {
        public List<string> Months { get; set; }
        public Dictionary<string, List<decimal>> Revenues { get; set; }
        public Dictionary<string, List<decimal>> Expenses { get; set; }
    }

}
