using DealerEcommerce.Application.Abstractions;
using DealerEcommerce.Application.Common;
using DealerEcommerce.Application.Dealers.Commands;
using DealerEcommerce.Application.Dealers.DTOs;

namespace DealerEcommerce.Application.Dealers.Handlers
{
    public class UpdateDealerHandler : ICommandHandler<UpdateDealerCommand, Result<DealerDto>>
    {
        private readonly IDealerRepository _dealerRepository;
        private readonly IUserRepository _userRepository;

        public UpdateDealerHandler(
            IDealerRepository dealerRepository,
            IUserRepository userRepository)
        {
            _dealerRepository = dealerRepository;
            _userRepository = userRepository;
        }

        public async Task<Result<DealerDto>> HandleAsync(
            UpdateDealerCommand command,
            CancellationToken cancellationToken = default)
        {
            if (command.DealerId == Guid.Empty)
                return Result<DealerDto>.Failure("El dealer es obligatorio.");

            if (!Enum.IsDefined(command.DealerType))
                return Result<DealerDto>.Failure("El tipo de dealer no es válido.");

            if (!Enum.IsDefined(command.DocumentType))
                return Result<DealerDto>.Failure("El tipo de documento no es válido.");

            if (string.IsNullOrWhiteSpace(command.DocumentNumber))
                return Result<DealerDto>.Failure("El número de documento es obligatorio.");

            if (string.IsNullOrWhiteSpace(command.Email))
                return Result<DealerDto>.Failure("El correo es obligatorio.");

            var dealer = await _dealerRepository.GetByIdAsync(command.DealerId, cancellationToken);

            if (dealer == null)
                return Result<DealerDto>.Failure("No existe el dealer indicado.");

            var existingDealer = await _dealerRepository.GetByDocumentNumberAsync(
                command.DocumentNumber,
                cancellationToken);

            if (existingDealer != null && existingDealer.Id != command.DealerId)
                return Result<DealerDto>.Failure("Ya existe un dealer con ese número de documento.");

            dealer.UpdateProfile(
                command.DealerType,
                command.RazonSocial,
                command.BusinessName,
                command.Email,
                command.DocumentType,
                command.DocumentNumber);

            await _dealerRepository.UpdateAsync(dealer, cancellationToken);

            var user = await _userRepository.GetByDealerIdAsync(dealer.Id, cancellationToken);

            var dto = new DealerDto
            {
                Id = dealer.Id,
                UserId = user?.Id ?? default,
                DealerType = dealer.DealerType,
                RazonSocial = dealer.RazonSocial,
                BusinessName = dealer.BusinessName,
                Email = dealer.Email,
                DocumentType = dealer.DocumentType,
                DocumentNumber = dealer.DocumentNumber
            };

            return Result<DealerDto>.Success(dto, "Dealer actualizado correctamente.");
        }
    }
}
