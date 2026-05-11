using DealerEcommerce.Application.Abstractions;
using DealerEcommerce.Application.Common;
using DealerEcommerce.Application.Dealers.Commands;
using DealerEcommerce.Application.Dealers.DTOs;
using DealerEcommerce.Domain.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DealerEcommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DealersController : ControllerBase
    {
        private readonly ICommandHandler<CreateDealerCommand, Result<DealerDto>> _createDealerHandler;
        private readonly ICommandHandler<AddDealerAddressCommand, Result<DealerAddressDto>> _addDealerAddressHandler;

        public DealersController(
            ICommandHandler<CreateDealerCommand, Result<DealerDto>> createDealerHandler,
            ICommandHandler<AddDealerAddressCommand, Result<DealerAddressDto>> addDealerAddressHandler)
        {
            _createDealerHandler = createDealerHandler;
            _addDealerAddressHandler = addDealerAddressHandler;
        }

        [HttpPost]
        [Authorize(Roles = nameof(UserRole.Admin))]
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
            command.DealerId = dealerId;

            var result = await _addDealerAddressHandler.HandleAsync(command, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
    }

}
