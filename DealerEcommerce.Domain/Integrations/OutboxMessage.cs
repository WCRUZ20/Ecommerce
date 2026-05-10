using DealerEcommerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealerEcommerce.Domain.Integrations
{
    public class OutboxMessage : BaseEntity
    {
        public string EventType { get; private set; } = string.Empty;

        public string Payload { get; private set; } = string.Empty;

        public bool Processed { get; private set; }

        public DateTime? ProcessedAt { get; private set; }

        public string? ErrorMessage { get; private set; }

        private OutboxMessage() { }

        public OutboxMessage(string eventType, string payload)
        {
            EventType = eventType;
            Payload = payload;
            Processed = false;
        }

        public void MarkAsProcessed()
        {
            Processed = true;
            ProcessedAt = DateTime.UtcNow;
            ErrorMessage = null;
            MarkAsUpdated();
        }

        public void MarkAsFailed(string errorMessage)
        {
            ErrorMessage = errorMessage;
            MarkAsUpdated();
        }
    }
}
