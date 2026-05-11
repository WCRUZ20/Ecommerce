using DealerEcommerce.Application.Abstractions;
using DealerEcommerce.Application.Common;
using DealerEcommerce.Application.Dealers.Commands;
using DealerEcommerce.Application.Dealers.DTOs;

namespace DealerEcommerce.Application.Dealers.Handlers
{
    public class UpdateDealerAddressHandler : ICommandHandler<UpdateDealerAddressCommand, Result<DealerAddressDto>>
    {
        private readonly IDealerRepository _dealerRepository;

        public UpdateDealerAddressHandler(IDealerRepository dealerRepository)
        {
            _dealerRepository = dealerRepository;
        }

        public async Task<Result<DealerAddressDto>> HandleAsync(
            UpdateDealerAddressCommand command,
            CancellationToken cancellationToken = default)
        {
            if (command.AddressId == Guid.Empty)
                return Result<DealerAddressDto>.Failure("La dirección es obligatoria.");

            if (command.DealerId == Guid.Empty)
                return Result<DealerAddressDto>.Failure("El dealer es obligatorio.");

            if (!Enum.IsDefined(command.AddressType))
                return Result<DealerAddressDto>.Failure("El tipo de dirección no es válido.");

            if (string.IsNullOrWhiteSpace(command.Province))
                return Result<DealerAddressDto>.Failure("La provincia es obligatoria.");

            if (string.IsNullOrWhiteSpace(command.City))
                return Result<DealerAddressDto>.Failure("La ciudad es obligatoria.");

            if (string.IsNullOrWhiteSpace(command.MainStreet))
                return Result<DealerAddressDto>.Failure("La calle principal es obligatoria.");

            var dealerExists = await _dealerRepository.ExistsAsync(command.DealerId, cancellationToken);

            if (!dealerExists)
                return Result<DealerAddressDto>.Failure("No existe el dealer indicado.");

            var address = await _dealerRepository.GetAddressByIdAsync(command.AddressId, cancellationToken);

            if (address == null)
                return Result<DealerAddressDto>.Failure("No existe la dirección indicada.");

            address.UpdateDetails(
                command.DealerId,
                command.AddressType,
                command.Province,
                command.City,
                command.MainStreet,
                command.SecondaryStreet,
                command.Reference,
                command.IsDefault);

            await _dealerRepository.UpdateAddressAsync(address, command.IsDefault, cancellationToken);

            var dto = new DealerAddressDto
            {
                Id = address.Id,
                DealerId = address.DealerId,
                AddressType = address.AddressType,
                Province = address.Province,
                City = address.City,
                MainStreet = address.MainStreet,
                SecondaryStreet = address.SecondaryStreet,
                Reference = address.Reference,
                IsDefault = address.IsDefault
            };

            return Result<DealerAddressDto>.Success(dto, "Dirección del dealer actualizada correctamente.");
        }
    }
}
