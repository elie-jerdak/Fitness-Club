using System;
using System.Collections.Generic;

namespace FitnessClub_Test.Core.NewModels;

public partial class AvailabilityBooking
{
    public int Id { get; set; }

    public string Type { get; set; }

    public string Status { get; set; }

    public DateOnly Date { get; set; }

    public bool IsDeleted { get; set; }

    public int AvailabilityId { get; set; }

    public int ClientId { get; set; }

    public virtual Availability Availability { get; set; }

    public virtual Client Client { get; set; }
}
