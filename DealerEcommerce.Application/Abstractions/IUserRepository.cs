using DealerEcommerce.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealerEcommerce.Application.Abstractions
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameOrEmailAsync(
            string usernameOrEmail,
            CancellationToken cancellationToken = default);

        Task<User?> GetByIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<User?> GetByDealerIdAsync(
            Guid dealerId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<User>> GetAllAsync(
            CancellationToken cancellationToken = default);

        Task AddAsync(
            User user,
            CancellationToken cancellationToken = default);

        Task UpdateAsync(
            User user,
            CancellationToken cancellationToken = default);
    }
}
