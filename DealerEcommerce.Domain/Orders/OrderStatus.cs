using System;
using System.Collections.Generic;
using System.Text;

namespace DealerEcommerce.Domain.Orders
{
    public enum OrderStatus
    {
        Created = 1,
        PendingIntegration = 2,
        SentToExternalSystem = 3,
        Confirmed = 4,
        Failed = 5,
        Cancelled = 6
    }
}
