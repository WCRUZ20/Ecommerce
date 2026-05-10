using System;
using System.Collections.Generic;
using System.Text;

namespace DealerEcommerce.Application.Dealers.DTOs
{
    public class DealerDto
    {
        public Guid Id { get; set; }

        public string DealerType { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string? SecondName { get; set; }

        public string LastName { get; set; } = string.Empty;

        public string? SecondLastName { get; set; }

        public string? BusinessName { get; set; }

        public string Email { get; set; } = string.Empty;

        public string DocumentType { get; set; } = string.Empty;

        public string DocumentNumber { get; set; } = string.Empty;
    }
}
