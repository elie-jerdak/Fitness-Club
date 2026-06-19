using System;
using System.Collections.Generic;

namespace FitnessClub_Test.Core.NewModels;

public partial class SubscriptionPayment
{
    public int Id { get; set; }

    public decimal? Amount { get; set; }

    public DateTime Date { get; set; }

    public string StripeSessionId { get; set; }

    public string Status { get; set; }

    public string Currency { get; set; }

    public string PaymentMethod { get; set; }

    public string FailureReason { get; set; }

    public string Type { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public int ClientId { get; set; }

    public virtual Client Client { get; set; }
}
