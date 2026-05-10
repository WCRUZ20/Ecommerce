using DealerEcommerce.Domain.Dealers;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealerEcommerce.Application.Dealers.Commands
{
    public class CreateDealerCommand
    {
        public Guid UserId { get; set; }

        public string? RazonSocial { get; set; }

        public string? BusinessName { get; set; }

        public string Email { get; set; } = string.Empty;

        public DocumentType DocumentType { get; set; }

        public string DocumentNumber { get; set; } = string.Empty;
    }
}
