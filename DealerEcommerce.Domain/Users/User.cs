using DealerEcommerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealerEcommerce.Domain.Users
{
    public class User : BaseEntity
    {
        public Guid? DealerId { get; private set; }

        public string Username { get; private set; } = string.Empty;

        public string FirstName { get; private set; } = string.Empty;

        public string? SecondName { get; private set; }

        public string LastName { get; private set; } = string.Empty;

        public string? SecondLastName { get; private set; }

        public string Email { get; private set; } = string.Empty;

        public string PasswordHash { get; private set; } = string.Empty;

        public UserRole Role { get; private set; }

        public UserDealerType DealerType { get; private set; }

        public bool MustChangePassword { get; private set; }

        private User() { }

        public User(
            Guid? dealerId,
            string username,
            string email,
            string passwordHash,
            string firstname,
            string? secondname,
            string lastName,
            string? secondlastname,
            UserRole role,
            UserDealerType dealerType)
        {
            DealerId = dealerId;
            Username = username;
            Email = email;
            PasswordHash = passwordHash;
            FirstName = firstname;
            SecondName = secondname;
            LastName = lastName;
            SecondLastName = secondlastname;
            Role = role;
            DealerType = dealerType;
            MustChangePassword = false;
        }

        public void AssignDealer(Guid dealerId)
        {
            DealerId = dealerId;
            MarkAsUpdated();
        }

        public void ChangePassword(string passwordHash)
        {
            PasswordHash = passwordHash;
            MustChangePassword = false;
            MarkAsUpdated();
        }
    }
}
