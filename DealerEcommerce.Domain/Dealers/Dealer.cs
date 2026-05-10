using DealerEcommerce.Domain.Addresses;
using DealerEcommerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace DealerEcommerce.Domain.Dealers
{
    public class Dealer : BaseEntity
    {
        public DealerType DealerType { get; private set; }

        //public string FirstName { get; private set; } = string.Empty;

        //public string? SecondName { get; private set; }

        //public string LastName { get; private set; } = string.Empty;

        //public string? SecondLastName { get; private set; }
        public string razonSocial { get; private set; }

        public string? BusinessName { get; private set; }

        public string Email { get; private set; } = string.Empty;

        public DocumentType DocumentType { get; private set; }

        public string DocumentNumber { get; private set; } = string.Empty;

        private readonly List<DealerAddress> _addresses = new();

        public IReadOnlyCollection<DealerAddress> Addresses => _addresses.AsReadOnly();

        private Dealer() { }

        public Dealer(
            DealerType dealerType,
            //string firstName,
            //string? secondName,
            //string lastName,
            //string? secondLastName,
            string razonsocial,
            string? businessName,
            string email,
            DocumentType documentType,
            string documentNumber)
        {
            DealerType = dealerType;
            //FirstName = firstName;
            //SecondName = secondName;
            //LastName = lastName;
            //SecondLastName = secondLastName;
            razonSocial = razonsocial;
            BusinessName = businessName;
            Email = email;
            DocumentType = documentType;
            DocumentNumber = documentNumber;
        }

        public void UpdateProfile(
            //string firstName,
            //string? secondName,
            //string lastName,
            //string? secondLastName,
            string? razonsocial,
            string? businessName,
            string email)
        {
            //FirstName = firstName;
            //SecondName = secondName;
            //LastName = lastName;
            //SecondLastName = secondLastName;
            razonSocial = razonsocial;
            BusinessName = businessName;
            Email = email;

            MarkAsUpdated();
        }

        public void AddAddress(DealerAddress address)
        {
            _addresses.Add(address);
            MarkAsUpdated();
        }
    }
}
