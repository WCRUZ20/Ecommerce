using DealerEcommerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealerEcommerce.Domain.Products
{
    public class Product : BaseEntity
    {
        public string Sku { get; private set; } = string.Empty;

        public string Name { get; private set; } = string.Empty;

        public string? Description { get; private set; }

        public decimal Price { get; private set; }

        public decimal StockQuantity { get; private set; }

        private Product() { }

        public Product(
            string sku,
            string name,
            string? description,
            decimal price,
            decimal stockQuantity)
        {
            Sku = sku;
            Name = name;
            Description = description;
            Price = price;
            StockQuantity = stockQuantity;
        }

        public void UpdateStock(decimal stockQuantity)
        {
            StockQuantity = stockQuantity;
            MarkAsUpdated();
        }

        public void UpdatePrice(decimal price)
        {
            Price = price;
            MarkAsUpdated();
        }
    }
}
