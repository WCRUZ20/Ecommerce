using DealerEcommerce.Domain.Dealers;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealerEcommerce.Application.Dealers.Commands
{
    public class CreateDealerCommand
    {
        public DealerType DealerType { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string? SecondName { get; set; }

        public string LastName { get; set; } = string.Empty;

        public string? SecondLastName { get; set; }

        public string? BusinessName { get; set; }

        public string Email { get; set; } = string.Empty;

        public DocumentType DocumentType { get; set; }

        public string DocumentNumber { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}
