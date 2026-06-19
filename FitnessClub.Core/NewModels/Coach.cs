using System;
using System.Collections.Generic;

namespace FitnessClub_Test.Core.NewModels;

public partial class Coach
{
    public int Id { get; set; }

    public int? Experience { get; set; }

    public string Specialty { get; set; }

    public decimal? Salary { get; set; }

    public string Bio { get; set; }

    public int UserId { get; set; }

    public virtual ICollection<Availability> Availabilities { get; set; } = new List<Availability>();

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();

    public virtual ICollection<PremadeProgram> PremadePrograms { get; set; } = new List<PremadeProgram>();

    public virtual ICollection<Feedback> Feedbacks { get; set; }
    public virtual User User { get; set; }
}
