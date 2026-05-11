using DealerEcommerce.Domain.Addresses;
using DealerEcommerce.Domain.Dealers;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealerEcommerce.Application.Abstractions
{
    public interface IDealerRepository
    {
        Task<Dealer?> GetByIdAsync(
            Guid dealerId,
            CancellationToken cancellationToken = default);

        Task<Dealer?> GetByDocumentNumberAsync(
            string documentNumber,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<Dealer>> GetAllAsync(
            CancellationToken cancellationToken = default);

        Task<DealerAddress?> GetAddressByIdAsync(
            Guid addressId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<DealerAddress>> GetAddressesAsync(
            CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<DealerAddress>> GetAddressesByDealerIdAsync(
            Guid dealerId,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(
            Guid dealerId,
            CancellationToken cancellationToken = default);

        Task AddAddressAsync(
            DealerAddress address,
            bool setAsDefault,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            Dealer dealer,
            CancellationToken cancellationToken = default);

        Task UpdateAsync(
            Dealer dealer,
            CancellationToken cancellationToken = default);
    }
}
