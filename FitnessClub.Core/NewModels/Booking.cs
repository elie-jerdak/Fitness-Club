using System;
using System.Collections.Generic;

namespace FitnessClub_Test.Core.NewModels;

public partial class Booking
{
    public int Id { get; set; }

    public string Type { get; set; }

    public string Status { get; set; }

    public DateTime? Date { get; set; }

    public bool IsDeleted { get; set; }

    public int ClientId { get; set; }

    public int ClassId { get; set; }

    public virtual Class Class { get; set; }

    public virtual Client Client { get; set; }
}
