using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessClub_Test.Dtos
{
    public class ClassDetailsDTO
    {
        public ClassDTO Class { get; set; }
        public List<EnrolledClientDTO> Clients { get; set; } // already enrolled

        public List<EnrolledClientDTO> AllClients { get; set; } // for dropdown
    }
}
