using DealerEcommerce.Application.Abstractions;
using DealerEcommerce.Domain.Users;
using DealerEcommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealerEcommerce.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DealerEcommerceDbContext _context;

        public UserRepository(DealerEcommerceDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByUsernameOrEmailAsync(
            string usernameOrEmail,
            CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x =>
                    x.Username == usernameOrEmail ||
                    x.Email == usernameOrEmail,
                    cancellationToken);
        }

        public async Task<User?> GetByIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        }

        public async Task<User?> GetByDealerIdAsync(
            Guid dealerId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x => x.DealerId == dealerId, cancellationToken);
        }

        public async Task<IReadOnlyCollection<User>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(
            User user,
            CancellationToken cancellationToken = default)
        {
            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(
            User user,
            CancellationToken cancellationToken = default)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
