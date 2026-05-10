using DealerEcommerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealerEcommerce.Domain.Integrations
{
    public class IntegrationLog : BaseEntity
    {
        public Guid? OrderId { get; private set; }

        public string IntegrationName { get; private set; } = string.Empty;

        public string Action { get; private set; } = string.Empty;

        public bool Success { get; private set; }

        public string? Request { get; private set; }

        public string? Response { get; private set; }

        public string? ErrorMessage { get; private set; }

        private IntegrationLog() { }

        public IntegrationLog(
            Guid? orderId,
            string integrationName,
            string action,
            bool success,
            string? request,
            string? response,
            string? errorMessage)
        {
            OrderId = orderId;
            IntegrationName = integrationName;
            Action = action;
            Success = success;
            Request = request;
            Response = response;
            ErrorMessage = errorMessage;
        }
    }
}
