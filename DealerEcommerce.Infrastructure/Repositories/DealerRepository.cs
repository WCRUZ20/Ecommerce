using DealerEcommerce.Application.Abstractions;
using DealerEcommerce.Domain.Addresses;
using DealerEcommerce.Domain.Dealers;
using DealerEcommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealerEcommerce.Infrastructure.Repositories
{
    public class DealerRepository : IDealerRepository
    {
        private readonly DealerEcommerceDbContext _context;

        public DealerRepository(DealerEcommerceDbContext context)
        {
            _context = context;
        }

        public async Task<Dealer?> GetByIdAsync(
            Guid dealerId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Dealers
                .Include(x => x.Addresses)
                .FirstOrDefaultAsync(x => x.Id == dealerId, cancellationToken);
        }

        public async Task<Dealer?> GetByDocumentNumberAsync(
            string documentNumber,
            CancellationToken cancellationToken = default)
        {
            return await _context.Dealers
                .FirstOrDefaultAsync(x => x.DocumentNumber == documentNumber, cancellationToken);
        }

        public async Task<bool> ExistsAsync(
            Guid dealerId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Dealers
                .AnyAsync(x => x.Id == dealerId, cancellationToken);
        }

        public async Task AddAddressAsync(
            DealerAddress address,
            bool setAsDefault,
            CancellationToken cancellationToken = default)
        {
            if (setAsDefault)
            {
                var currentDefaultAddresses = await _context.DealerAddresses
                    .Where(x => x.DealerId == address.DealerId && x.IsDefault)
                    .ToListAsync(cancellationToken);

                foreach (var currentDefaultAddress in currentDefaultAddresses)
                {
                    currentDefaultAddress.RemoveDefault();
                }
            }

            await _context.DealerAddresses.AddAsync(address, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task AddAsync(
            Dealer dealer,
            CancellationToken cancellationToken = default)
        {
            await _context.Dealers.AddAsync(dealer, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(
            Dealer dealer,
            CancellationToken cancellationToken = default)
        {
            if (_context.Entry(dealer).State == EntityState.Detached)
            {
                _context.Dealers.Update(dealer);
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
