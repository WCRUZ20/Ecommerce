using DealerEcommerce.Domain.Dealers;
using DealerEcommerce.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealerEcommerce.Application.Users.Commands
{
    public class CreateUserCommand
    {
        public UserRole Role { get; set; }

        public UserDealerType DealerType { get; set; } = UserDealerType.NoAplica;

        public string FirstName { get; set; } = string.Empty;

        public string? SecondName { get; set; }

        public string LastName { get; set; } = string.Empty;

        public string? SecondLastName { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}
