using DealerEcommerce.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealerEcommerce.Application.Abstractions
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user);
    }
}
