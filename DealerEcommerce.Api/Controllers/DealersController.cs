using DealerEcommerce.Application.Abstractions;
using DealerEcommerce.Application.Common;
using DealerEcommerce.Application.Dealers.Commands;
using DealerEcommerce.Application.Dealers.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DealerEcommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DealersController : ControllerBase
    {
        private readonly ICommandHandler<CreateDealerCommand, Result<DealerDto>> _createDealerHandler;

        public DealersController(
            ICommandHandler<CreateDealerCommand, Result<DealerDto>> createDealerHandler)
        {
            _createDealerHandler = createDealerHandler;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create(
            [FromBody] CreateDealerCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _createDealerHandler.HandleAsync(command, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
