using DealerEcommerce.Application.Abstractions;
using DealerEcommerce.Application.Auth.Commands;
using DealerEcommerce.Application.Auth.DTOs;
using DealerEcommerce.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace DealerEcommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ICommandHandler<LoginCommand, Result<LoginResponseDto>> _loginHandler;

        public AuthController(
            ICommandHandler<LoginCommand, Result<LoginResponseDto>> loginHandler)
        {
            _loginHandler = loginHandler;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _loginHandler.HandleAsync(command, cancellationToken);

            if (!result.IsSuccess)
                return Unauthorized(result);

            return Ok(result);
        }
    }
}
