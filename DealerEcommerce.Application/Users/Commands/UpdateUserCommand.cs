using DealerEcommerce.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealerEcommerce.Application.Users.Commands
{
    public class UpdateUserCommand
    {
        public Guid UserId { get; set; }

        public Guid? DealerId { get; set; }

        public UserRole Role { get; set; }

        public UserDealerType DealerType { get; set; } = UserDealerType.NoAplica;

        public string FirstName { get; set; } = string.Empty;

        public string? SecondName { get; set; }

        public string LastName { get; set; } = string.Empty;

        public string? SecondLastName { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;
    }
}
