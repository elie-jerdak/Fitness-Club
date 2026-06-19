using System;
using System.Collections.Generic;

namespace FitnessClub_Test.Core.NewModels;

public partial class Client
{
    public int Id { get; set; }

    public decimal? Height { get; set; }

    public decimal? Weight { get; set; }

    public string Target { get; set; }

    public string MedicalHistory { get; set; }

    public int UserId { get; set; }

    public virtual ICollection<AvailabilityBooking> AvailabilityBookings { get; set; } = new List<AvailabilityBooking>();

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Membership> Memberships { get; set; } = new List<Membership>();

    public virtual ICollection<SubscriptionPayment> SubscriptionPayments { get; set; } = new List<SubscriptionPayment>();

    public virtual User User { get; set; }

    public virtual ICollection<WorkoutLog> WorkoutLogs { get; set; } = new List<WorkoutLog>();

    public virtual ICollection<PremadeProgram> PremadePrograms { get; set; } = new List<PremadeProgram>();
    public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
}
