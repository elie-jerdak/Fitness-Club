using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessClub_Test.Core.NewModels;

public partial class User : IdentityUser<int>
{
    //refresh tokens
    public string RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public DateOnly Dob { get; set; }

    public string Gender { get; set; }

    public string Address { get; set; }

    public string Photo { get; set; }

    public DateTime? DateCreated { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public string QrCode { get; set; }


    // Calendar navigation
    public virtual ICollection<Calendar> AttendingEvents { get; set; } = new List<Calendar>();

    // Optional: If user is a coach, these are the events they coach
    public virtual ICollection<Calendar> CoachEvents { get; set; } = new List<Calendar>();

    // Optional: If user is a client, these are their assigned events
    public virtual ICollection<Calendar> ClientEvents { get; set; } = new List<Calendar>();

    
    public virtual ICollection<CheckingInOut> CheckingInOuts { get; set; } = new List<CheckingInOut>();

    public virtual ICollection<Client> Clients { get; set; } = new List<Client>();

    public virtual ICollection<Coach> Coaches { get; set; } = new List<Coach>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
