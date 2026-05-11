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

        public async Task<IReadOnlyCollection<Dealer>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.Dealers
                .Include(x => x.Addresses)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<DealerAddress?> GetAddressByIdAsync(
            Guid addressId,
            CancellationToken cancellationToken = default)
        {
            return await _context.DealerAddresses
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == addressId, cancellationToken);
        }

        public async Task<IReadOnlyCollection<DealerAddress>> GetAddressesAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.DealerAddresses
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyCollection<DealerAddress>> GetAddressesByDealerIdAsync(
            Guid dealerId,
            CancellationToken cancellationToken = default)
        {
            return await _context.DealerAddresses
                .AsNoTracking()
                .Where(x => x.DealerId == dealerId)
                .ToListAsync(cancellationToken);
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


        public async Task UpdateAddressAsync(
            DealerAddress address,
            bool setAsDefault,
            CancellationToken cancellationToken = default)
        {
            if (setAsDefault)
            {
                var currentDefaultAddresses = await _context.DealerAddresses
                    .Where(x =>
                        x.DealerId == address.DealerId &&
                        x.Id != address.Id &&
                        x.IsDefault)
                    .ToListAsync(cancellationToken);

                foreach (var currentDefaultAddress in currentDefaultAddresses)
                {
                    currentDefaultAddress.RemoveDefault();
                }
            }

            _context.DealerAddresses.Update(address);
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
