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

        Task AddAsync(
            Dealer dealer,
            CancellationToken cancellationToken = default);

        Task UpdateAsync(
            Dealer dealer,
            CancellationToken cancellationToken = default);
    }
}
