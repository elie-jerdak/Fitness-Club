using System;
using System.Collections.Generic;

namespace FitnessClub_Test.Core.NewModels;

public partial class PremadeProgram
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public string CoverImage { get; set; }

    public bool IsPaid { get; set; }

    public decimal? Price { get; set; }

    public string Category { get; set; }

    public int? Duration { get; set; }

    public string EquipmentNeeded { get; set; } 
    public string Level { get; set; }
    public string Intensity { get; set; }
    public int NumberOfExercises { get; set; }
    public string Benefits { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public int? CoachId { get; set; }

    public virtual Coach Coach { get; set; }

    public virtual ICollection<Client> Clients { get; set; } = new List<Client>();
    public ICollection<PremadeProgramExercise> PremadeProgramExercises { get; set; }
}
