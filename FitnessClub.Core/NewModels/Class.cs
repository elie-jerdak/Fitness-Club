using System;
using System.Collections.Generic;

namespace FitnessClub_Test.Core.NewModels;

public partial class Class
{
    public int Id { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndTime { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string Status { get; set; }

    public string Reccurence { get; set; }

    public string Type { get; set; }

    public decimal Fee { get; set; }

    public int? MaxOccupancy { get; set; }

    public string Feedback { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public int CoachId { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Coach Coach { get; set; }
}
