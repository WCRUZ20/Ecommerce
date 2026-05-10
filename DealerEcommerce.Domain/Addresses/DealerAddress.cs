using DealerEcommerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealerEcommerce.Domain.Addresses
{
    public class DealerAddress : BaseEntity
    {
        public Guid DealerId { get; private set; }

        public AddressType AddressType { get; private set; }

        public string Province { get; private set; } = string.Empty;

        public string City { get; private set; } = string.Empty;

        public string MainStreet { get; private set; } = string.Empty;

        public string? SecondaryStreet { get; private set; }

        public string? Reference { get; private set; }

        public bool IsDefault { get; private set; }

        private DealerAddress() { }

        public DealerAddress(
            Guid dealerId,
            AddressType addressType,
            string province,
            string city,
            string mainStreet,
            string? secondaryStreet,
            string? reference,
            bool isDefault)
        {
            DealerId = dealerId;
            AddressType = addressType;
            Province = province;
            City = city;
            MainStreet = mainStreet;
            SecondaryStreet = secondaryStreet;
            Reference = reference;
            IsDefault = isDefault;
        }

        public void SetAsDefault()
        {
            IsDefault = true;
            MarkAsUpdated();
        }

        public void RemoveDefault()
        {
            IsDefault = false;
            MarkAsUpdated();
        }
    }
}
