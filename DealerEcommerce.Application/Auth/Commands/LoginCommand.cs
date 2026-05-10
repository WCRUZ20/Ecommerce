using System;
using System.Collections.Generic;
using System.Text;

namespace DealerEcommerce.Application.Auth.Commands
{
    public class LoginCommand
    {
        public string UsernameOrEmail { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}
