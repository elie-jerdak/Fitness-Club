using System;
using System.Collections.Generic;

namespace FitnessClub_Test.Core.NewModels;

public partial class Availability
{
    public int Id { get; set; }

    public DateOnly Day { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public int CoachId { get; set; }

    public virtual ICollection<AvailabilityBooking> AvailabilityBookings { get; set; } = new List<AvailabilityBooking>();

    public virtual Coach Coach { get; set; }
}
