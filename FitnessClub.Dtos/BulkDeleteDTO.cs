using System.Collections.Generic;

namespace FitnessClub_Test.Dtos
{
    public class BulkDeleteDTO
    {
        public string Message { get; set; } = "";
        public int DeletedCount { get; set; }
        public int FailedCount { get; set; }
        public List<int> FailedUserIds { get; set; } = new();
    }
}
