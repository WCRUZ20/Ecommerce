using DealerEcommerce.Domain.Addresses;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealerEcommerce.Application.Dealers.Commands
{
    public class AddDealerAddressCommand
    {
        public Guid DealerId { get; set; }

        public AddressType AddressType { get; set; }

        public string Province { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string MainStreet { get; set; } = string.Empty;

        public string? SecondaryStreet { get; set; }

        public string? Reference { get; set; }

        public bool IsDefault { get; set; }
    }
}
