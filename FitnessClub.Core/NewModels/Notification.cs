using System;
using System.Collections.Generic;

namespace FitnessClub_Test.Core.NewModels;

public partial class Notification
{
    public int Id { get; set; }

    public string Message { get; set; }

    public string Type { get; set; }

    public DateTime Time { get; set; }

    public bool IsGlobal { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
