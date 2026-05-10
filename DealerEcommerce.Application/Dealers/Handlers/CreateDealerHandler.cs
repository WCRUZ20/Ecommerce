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

        public CreateDealerHandler(
            IDealerRepository dealerRepository,
            IUserRepository userRepository)
        {
            _dealerRepository = dealerRepository;
            _userRepository = userRepository;
        }

        public async Task<Result<DealerDto>> HandleAsync(
            CreateDealerCommand command,
            CancellationToken cancellationToken = default)
        {
            if (command.UserId == Guid.Empty)
                return Result<DealerDto>.Failure("El usuario es obligatorio para crear un dealer.");

            var user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);

            if (user == null)
                return Result<DealerDto>.Failure("No existe el usuario indicado.");

            if (user.Role != UserRole.Dealer)
                return Result<DealerDto>.Failure("Solo los usuarios con UserRole = 3 pueden ser creados como dealer.");

            if (user.DealerId.HasValue)
                return Result<DealerDto>.Failure("El usuario ya tiene un dealer asociado.");

            if (!Enum.IsDefined(user.DealerType) || user.DealerType == UserDealerType.NoAplica)
                return Result<DealerDto>.Failure("El usuario dealer debe tener un tipo de dealer válido.");

            if (!Enum.IsDefined(command.DocumentType))
                return Result<DealerDto>.Failure("El tipo de documento no es válido.");

            if (string.IsNullOrWhiteSpace(command.DocumentNumber))
                return Result<DealerDto>.Failure("El número de documento es obligatorio para crear un dealer.");

            if (string.IsNullOrWhiteSpace(command.Email))
                return Result<DealerDto>.Failure("El correo es obligatorio para crear un dealer.");

            var existingDealer = await _dealerRepository.GetByDocumentNumberAsync(
                command.DocumentNumber,
                cancellationToken);

            if (existingDealer != null)
                return Result<DealerDto>.Failure("Ya existe un dealer con ese número de documento.");

            var dealerType = user.DealerType == UserDealerType.NaturalPerson
                ? DealerType.NaturalPerson
                : DealerType.LegalEntity;

            var dealer = new Dealer(
                dealerType,
                command.RazonSocial,
                command.BusinessName,
                command.Email,
                command.DocumentType,
                command.DocumentNumber);

            await _dealerRepository.AddAsync(dealer, cancellationToken);

            user.AssignDealer(dealer.Id);
            await _userRepository.UpdateAsync(user, cancellationToken);

            var dto = new DealerDto
            {
                Id = dealer.Id,
                UserId = user.Id,
                DealerType = dealer.DealerType,
                RazonSocial = dealer.RazonSocial,
                BusinessName = dealer.BusinessName,
                Email = dealer.Email,
                DocumentType = dealer.DocumentType,
                DocumentNumber = dealer.DocumentNumber
            };

            return Result<DealerDto>.Success(dto, "Dealer creado correctamente.");
        }
    }
}
