using DealerEcommerce.Application.Abstractions;
using DealerEcommerce.Application.Common;
using DealerEcommerce.Application.Dealers.Commands;
using DealerEcommerce.Application.Dealers.DTOs;
using DealerEcommerce.Domain.Addresses;
using DealerEcommerce.Domain.Dealers;
using DealerEcommerce.Domain.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DealerEcommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DealersController : ControllerBase
    {
        private readonly ICommandHandler<CreateDealerCommand, Result<DealerDto>> _createDealerHandler;
        private readonly ICommandHandler<UpdateDealerCommand, Result<DealerDto>> _updateDealerHandler;
        private readonly ICommandHandler<AddDealerAddressCommand, Result<DealerAddressDto>> _addDealerAddressHandler;
        private readonly ICommandHandler<UpdateDealerAddressCommand, Result<DealerAddressDto>> _updateDealerAddressHandler;
        private readonly IDealerRepository _dealerRepository;
        private readonly IUserRepository _userRepository;

        public DealersController(
            ICommandHandler<CreateDealerCommand, Result<DealerDto>> createDealerHandler,
            ICommandHandler<UpdateDealerCommand, Result<DealerDto>> updateDealerHandler,
            ICommandHandler<AddDealerAddressCommand, Result<DealerAddressDto>> addDealerAddressHandler,
            ICommandHandler<UpdateDealerAddressCommand, Result<DealerAddressDto>> updateDealerAddressHandler,
            IDealerRepository dealerRepository,
            IUserRepository userRepository)
        {
            _createDealerHandler = createDealerHandler;
            _updateDealerHandler = updateDealerHandler;
            _addDealerAddressHandler = addDealerAddressHandler;
            _updateDealerAddressHandler = updateDealerAddressHandler;
            _dealerRepository = dealerRepository;
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            if (IsAdminOrDev())
            {
                var dealers = await _dealerRepository.GetAllAsync(cancellationToken);
                var dealerDtos = new List<DealerDto>();

                foreach (var _dealer in dealers)
                {
                    dealerDtos.Add(await MapToDtoAsync(_dealer, cancellationToken));
                }

                return Ok(Result<IReadOnlyCollection<DealerDto>>.Success(
                    dealerDtos,
                    "Dealers obtenidos correctamente."));
            }

            var currentDealerId = GetCurrentDealerId();

            if (!currentDealerId.HasValue)
                return Forbid();

            var dealer = await _dealerRepository.GetByIdAsync(currentDealerId.Value, cancellationToken);

            if (dealer == null)
                return NotFound(Result<DealerDto>.Failure("No existe el dealer asociado al usuario autenticado."));

            return Ok(Result<IReadOnlyCollection<DealerDto>>.Success(
                new List<DealerDto> { await MapToDtoAsync(dealer, cancellationToken) },
                "Dealer obtenido correctamente."));
        }

        [HttpGet("{dealerId:guid}")]
        public async Task<IActionResult> GetById(
            Guid dealerId,
            CancellationToken cancellationToken)
        {
            if (!IsAdminOrDev() && GetCurrentDealerId() != dealerId)
                return Forbid();

            var dealer = await _dealerRepository.GetByIdAsync(dealerId, cancellationToken);

            if (dealer == null)
                return NotFound(Result<DealerDto>.Failure("No existe el dealer indicado."));

            return Ok(Result<DealerDto>.Success(
                await MapToDtoAsync(dealer, cancellationToken),
                "Dealer obtenido correctamente."));
        }

        [HttpGet("addresses")]
        public async Task<IActionResult> GetAddresses(CancellationToken cancellationToken)
        {
            IReadOnlyCollection<DealerAddress> addresses;

            if (IsAdminOrDev())
            {
                addresses = await _dealerRepository.GetAddressesAsync(cancellationToken);
            }
            else
            {
                var currentDealerId = GetCurrentDealerId();

                if (!currentDealerId.HasValue)
                    return Forbid();

                addresses = await _dealerRepository.GetAddressesByDealerIdAsync(
                    currentDealerId.Value,
                    cancellationToken);
            }

            return Ok(Result<IReadOnlyCollection<DealerAddressDto>>.Success(
                addresses.Select(MapToDto).ToList(),
                "Direcciones de dealers obtenidas correctamente."));
        }

        [HttpGet("addresses/{addressId:guid}")]
        public async Task<IActionResult> GetAddressById(
            Guid addressId,
            CancellationToken cancellationToken)
        {
            var address = await _dealerRepository.GetAddressByIdAsync(addressId, cancellationToken);

            if (address == null)
                return NotFound(Result<DealerAddressDto>.Failure("No existe la dirección indicada."));

            if (!IsAdminOrDev() && GetCurrentDealerId() != address.DealerId)
                return Forbid();

            return Ok(Result<DealerAddressDto>.Success(
                MapToDto(address),
                "Dirección del dealer obtenida correctamente."));
        }

        [HttpGet("{dealerId:guid}/addresses")]
        public async Task<IActionResult> GetAddressesByDealerId(
            Guid dealerId,
            CancellationToken cancellationToken)
        {
            if (!IsAdminOrDev() && GetCurrentDealerId() != dealerId)
                return Forbid();

            var addresses = await _dealerRepository.GetAddressesByDealerIdAsync(dealerId, cancellationToken);

            return Ok(Result<IReadOnlyCollection<DealerAddressDto>>.Success(
                addresses.Select(MapToDto).ToList(),
                "Direcciones del dealer obtenidas correctamente."));
        }

        [HttpPut("{dealerId:guid}")]
        public async Task<IActionResult> Update(
            Guid dealerId,
            [FromBody] UpdateDealerCommand command,
            CancellationToken cancellationToken)
        {
            if (!IsAdminOrDev() && GetCurrentDealerId() != dealerId)
                return Forbid();

            var dealer = await _dealerRepository.GetByIdAsync(dealerId, cancellationToken);

            if (dealer == null)
                return NotFound(Result<DealerDto>.Failure("No existe el dealer indicado."));

            command.DealerId = dealerId;

            var result = await _updateDealerHandler.HandleAsync(command, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPut("addresses/{addressId:guid}")]
        public async Task<IActionResult> UpdateAddress(
            Guid addressId,
            [FromBody] UpdateDealerAddressCommand command,
            CancellationToken cancellationToken)
        {
            var address = await _dealerRepository.GetAddressByIdAsync(addressId, cancellationToken);

            if (address == null)
                return NotFound(Result<DealerAddressDto>.Failure("No existe la dirección indicada."));

            if (!IsAdminOrDev())
            {
                var currentDealerId = GetCurrentDealerId();

                if (currentDealerId != address.DealerId)
                    return Forbid();

                command.DealerId = currentDealerId.Value;
            }
            else if (command.DealerId == Guid.Empty)
            {
                command.DealerId = address.DealerId;
            }

            command.AddressId = addressId;

            var result = await _updateDealerAddressHandler.HandleAsync(command, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Dev)}")]
        public async Task<IActionResult> Create(
            [FromBody] CreateDealerCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _createDealerHandler.HandleAsync(command, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("{dealerId:guid}/addresses")]
        public async Task<IActionResult> AddAddress(
            Guid dealerId,
            [FromBody] AddDealerAddressCommand command,
            CancellationToken cancellationToken)
        {
            if (!IsAdminOrDev() && GetCurrentDealerId() != dealerId)
                return Forbid();

            command.DealerId = dealerId;

            var result = await _addDealerAddressHandler.HandleAsync(command, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        private bool IsAdminOrDev()
        {
            return User.IsInRole(nameof(UserRole.Admin)) || User.IsInRole(nameof(UserRole.Dev));
        }

        private Guid? GetCurrentDealerId()
        {
            var dealerId = User.FindFirstValue("dealerId");

            return Guid.TryParse(dealerId, out var parsedDealerId)
                ? parsedDealerId
                : null;
        }

        private static DealerDto MapToDto(Dealer dealer, Guid userId = default)
        {
            return new DealerDto
            {
                Id = dealer.Id,
                UserId = userId,
                DealerType = dealer.DealerType,
                RazonSocial = dealer.RazonSocial,
                BusinessName = dealer.BusinessName,
                Email = dealer.Email,
                DocumentType = dealer.DocumentType,
                DocumentNumber = dealer.DocumentNumber
            };
        }

        private async Task<DealerDto> MapToDtoAsync(
            Dealer dealer,
            CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByDealerIdAsync(dealer.Id, cancellationToken);
            return MapToDto(dealer, user?.Id ?? default);
        }

        private static DealerAddressDto MapToDto(DealerAddress address)
        {
            return new DealerAddressDto
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
        }
    }
}
