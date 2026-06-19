using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessClub_Test.Core.Interfaces
{
    public interface IPaymentService
    {
        Task HandleCheckoutSessionCompletedAsync(Session session);
    }
}
