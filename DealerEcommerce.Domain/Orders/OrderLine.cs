using DealerEcommerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealerEcommerce.Domain.Orders
{
    public class OrderLine : BaseEntity
    {
        public Guid OrderId { get; private set; }

        public Guid ProductId { get; private set; }

        public string Sku { get; private set; } = string.Empty;

        public string ProductName { get; private set; } = string.Empty;

        public decimal Quantity { get; private set; }

        public decimal UnitPrice { get; private set; }

        public decimal LineTotal { get; private set; }

        private OrderLine() { }

        public OrderLine(
            Guid productId,
            string sku,
            string productName,
            decimal quantity,
            decimal unitPrice)
        {
            ProductId = productId;
            Sku = sku;
            ProductName = productName;
            Quantity = quantity;
            UnitPrice = unitPrice;
            LineTotal = quantity * unitPrice;
        }
    }
}
