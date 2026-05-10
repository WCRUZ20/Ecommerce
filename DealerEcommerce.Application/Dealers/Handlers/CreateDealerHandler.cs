using DealerEcommerce.Application.Abstractions;
using DealerEcommerce.Application.Common;
using DealerEcommerce.Application.Dealers.Commands;
using DealerEcommerce.Application.Dealers.DTOs;
using DealerEcommerce.Domain.Dealers;
using DealerEcommerce.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealerEcommerce.Application.Dealers.Handlers
{
    public class CreateDealerHandler : ICommandHandler<CreateDealerCommand, Result<DealerDto>>
    {
        private readonly IDealerRepository _dealerRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public CreateDealerHandler(
            IDealerRepository dealerRepository,
            IUserRepository userRepository,
            IPasswordHasher passwordHasher)
        {
            _dealerRepository = dealerRepository;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<Result<DealerDto>> HandleAsync(
            CreateDealerCommand command,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(command.Email))
                return Result<DealerDto>.Failure("El correo es obligatorio.");

            if (string.IsNullOrWhiteSpace(command.DocumentNumber))
                return Result<DealerDto>.Failure("El número de documento es obligatorio.");

            if (string.IsNullOrWhiteSpace(command.Username))
                return Result<DealerDto>.Failure("El usuario es obligatorio.");

            if (string.IsNullOrWhiteSpace(command.Password))
                return Result<DealerDto>.Failure("La contraseña es obligatoria.");

            var existingDealer = await _dealerRepository.GetByDocumentNumberAsync(
                command.DocumentNumber,
                cancellationToken);

            if (existingDealer != null)
                return Result<DealerDto>.Failure("Ya existe un dealer con ese número de documento.");

            var existingUser = await _userRepository.GetByUsernameOrEmailAsync(
                command.Username,
                cancellationToken);

            if (existingUser != null)
                return Result<DealerDto>.Failure("Ya existe un usuario con ese username.");

            var dealer = new Dealer(
                command.DealerType,
                command.FirstName,
                command.SecondName,
                command.LastName,
                command.SecondLastName,
                command.BusinessName,
                command.Email,
                command.DocumentType,
                command.DocumentNumber);

            await _dealerRepository.AddAsync(dealer, cancellationToken);

            var passwordHash = _passwordHasher.Hash(command.Password);

            var user = new User(
                dealer.Id,
                command.Username,
                command.Email,
                passwordHash,
                "Dealer");

            await _userRepository.AddAsync(user, cancellationToken);

            var dto = new DealerDto
            {
                Id = dealer.Id,
                DealerType = dealer.DealerType.ToString(),
                FirstName = dealer.FirstName,
                SecondName = dealer.SecondName,
                LastName = dealer.LastName,
                SecondLastName = dealer.SecondLastName,
                BusinessName = dealer.BusinessName,
                Email = dealer.Email,
                DocumentType = dealer.DocumentType.ToString(),
                DocumentNumber = dealer.DocumentNumber
            };

            return Result<DealerDto>.Success(dto, "Dealer creado correctamente.");
        }
    }
}
