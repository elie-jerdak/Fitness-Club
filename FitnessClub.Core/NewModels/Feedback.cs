using FitnessClub_Test.Core.NewModels;
using System;

public class Feedback
{
    public int Id { get; set; }

    public int CoachId { get; set; }
    public Coach Coach { get; set; }

    public int ClientId { get; set; }
    public Client Client { get; set; }

    public string Comment { get; set; }

    public DateTime CreatedAt { get; set; }
}
