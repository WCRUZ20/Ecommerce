using DealerEcommerce.Application.Abstractions;
using DealerEcommerce.Application.Common;
using DealerEcommerce.Application.Users.Commands;
using DealerEcommerce.Application.Users.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DealerEcommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ICommandHandler<CreateUserCommand, Result<UserDto>> _createUserHandler;

        public UsersController(
            ICommandHandler<CreateUserCommand, Result<UserDto>> createUserHandler)
        {
            _createUserHandler = createUserHandler;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create(
            [FromBody] CreateUserCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _createUserHandler.HandleAsync(command, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
    }

}
