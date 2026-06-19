using System;
using System.Collections.Generic;

namespace FitnessClub_Test.Core.NewModels;

public partial class Membership
{
    public int Id { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public string Type { get; set; }

    public bool IsAutorenewed { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public int ClientId { get; set; }

    public virtual Client Client { get; set; }
}
