using System;
using System.Collections.Generic;

namespace FitnessClub_Test.Core.NewModels;

public partial class WorkoutLog
{
    public int Id { get; set; }

    public DateOnly Date { get; set; }

    public string Type { get; set; }

    public int? Duration { get; set; }

    public string Notes { get; set; }

    public DateTime DateCreated { get; set; }

    public bool IsDeleted { get; set; }

    public int ClientId { get; set; }

    public virtual Client Client { get; set; }
}
