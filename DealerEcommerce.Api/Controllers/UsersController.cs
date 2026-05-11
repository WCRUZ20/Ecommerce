using DealerEcommerce.Application.Abstractions;
using DealerEcommerce.Application.Common;
using DealerEcommerce.Application.Users.Commands;
using DealerEcommerce.Application.Users.DTOs;
using DealerEcommerce.Domain.Users;
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
        [Authorize(Roles = nameof(UserRole.Admin))]
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
