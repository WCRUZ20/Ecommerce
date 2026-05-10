using DealerEcommerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealerEcommerce.Domain.Orders
{
    public class Order : BaseEntity
    {
        public Guid DealerId { get; private set; }

        public Guid ShippingAddressId { get; private set; }

        public Guid BillingAddressId { get; private set; }

        public OrderStatus Status { get; private set; } = OrderStatus.Created;

        public decimal Subtotal { get; private set; }

        public decimal TaxTotal { get; private set; }

        public decimal Total { get; private set; }

        private readonly List<OrderLine> _lines = new();

        public IReadOnlyCollection<OrderLine> Lines => _lines.AsReadOnly();

        private Order() { }

        public Order(
            Guid dealerId,
            Guid shippingAddressId,
            Guid billingAddressId)
        {
            DealerId = dealerId;
            ShippingAddressId = shippingAddressId;
            BillingAddressId = billingAddressId;
            Status = OrderStatus.Created;
        }

        public void AddLine(OrderLine line)
        {
            _lines.Add(line);
            RecalculateTotals();
        }

        public void MarkAsPendingIntegration()
        {
            Status = OrderStatus.PendingIntegration;
            MarkAsUpdated();
        }

        public void MarkAsSent()
        {
            Status = OrderStatus.SentToExternalSystem;
            MarkAsUpdated();
        }

        public void MarkAsFailed()
        {
            Status = OrderStatus.Failed;
            MarkAsUpdated();
        }

        private void RecalculateTotals()
        {
            Subtotal = _lines.Sum(x => x.LineTotal);
            TaxTotal = Math.Round(Subtotal * 0.15m, 2);
            Total = Subtotal + TaxTotal;

            MarkAsUpdated();
        }
    }
}
