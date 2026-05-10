using DealerEcommerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealerEcommerce.Domain.Users
{
    public class User : BaseEntity
    {
        public Guid DealerId { get; private set; }

        public string Username { get; private set; } = string.Empty;

        public string Email { get; private set; } = string.Empty;

        public string PasswordHash { get; private set; } = string.Empty;

        public string Role { get; private set; } = string.Empty;

        public bool MustChangePassword { get; private set; }

        private User() { }

        public User(
            Guid dealerId,
            string username,
            string email,
            string passwordHash,
            string role)
        {
            DealerId = dealerId;
            Username = username;
            Email = email;
            PasswordHash = passwordHash;
            Role = role;
            MustChangePassword = false;
        }

        public void ChangePassword(string passwordHash)
        {
            PasswordHash = passwordHash;
            MustChangePassword = false;
            MarkAsUpdated();
        }
    }
}
