using System;
using System.Collections.Generic;

namespace FitnessClub_Test.Core.NewModels;

public partial class Message
{
    public int Id { get; set; }

    public string Type { get; set; }

    public string Title { get; set; }

    public string Photo { get; set; }

    public string Content { get; set; }

    public DateTime Time { get; set; }

    public string Status { get; set; }

    public string AdminResponse { get; set; }

    public bool IsDeleted { get; set; }

    public int UserId { get; set; }

    public virtual User User { get; set; }
}
