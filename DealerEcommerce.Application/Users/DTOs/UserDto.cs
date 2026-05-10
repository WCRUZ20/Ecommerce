using System;
using System.Collections.Generic;
using System.Text;

namespace DealerEcommerce.Application.Users.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }

        public Guid? DealerId { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string DealerType { get; set; } = string.Empty;
    }
}
