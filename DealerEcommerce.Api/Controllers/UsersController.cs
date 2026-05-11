using DealerEcommerce.Application.Abstractions;
using DealerEcommerce.Application.Common;
using DealerEcommerce.Application.Users.Commands;
using DealerEcommerce.Application.Users.DTOs;
using DealerEcommerce.Domain.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DealerEcommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ICommandHandler<CreateUserCommand, Result<UserDto>> _createUserHandler;
        private readonly ICommandHandler<UpdateUserCommand, Result<UserDto>> _updateUserHandler;
        private readonly IUserRepository _userRepository;

        public UsersController(
            ICommandHandler<CreateUserCommand, Result<UserDto>> createUserHandler,
            ICommandHandler<UpdateUserCommand, Result<UserDto>> updateUserHandler,
            IUserRepository userRepository)
        {
            _createUserHandler = createUserHandler;
            _updateUserHandler = updateUserHandler;
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            if (IsAdminOrDev())
            {
                var users = await _userRepository.GetAllAsync(cancellationToken);
                return Ok(Result<IReadOnlyCollection<UserDto>>.Success(
                    users.Select(MapToDto).ToList(),
                    "Usuarios obtenidos correctamente."));
            }

            var currentUserId = GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Forbid();

            var currentUser = await _userRepository.GetByIdAsync(currentUserId.Value, cancellationToken);

            if (currentUser == null)
                return NotFound(Result<UserDto>.Failure("No existe el usuario autenticado."));

            return Ok(Result<IReadOnlyCollection<UserDto>>.Success(
                new List<UserDto> { MapToDto(currentUser) },
                "Usuario obtenido correctamente."));
        }

        [HttpGet("{userId:guid}")]
        public async Task<IActionResult> GetById(
            Guid userId,
            CancellationToken cancellationToken)
        {
            var currentUserId = GetCurrentUserId();

            if (!IsAdminOrDev() && currentUserId != userId)
                return Forbid();

            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

            if (user == null)
                return NotFound(Result<UserDto>.Failure("No existe el usuario indicado."));

            return Ok(Result<UserDto>.Success(MapToDto(user), "Usuario obtenido correctamente."));
        }

        [HttpPut("{userId:guid}")]
        public async Task<IActionResult> Update(
            Guid userId,
            [FromBody] UpdateUserCommand command,
            CancellationToken cancellationToken)
        {
            var isAdminOrDev = IsAdminOrDev();
            var currentUserId = GetCurrentUserId();

            if (!isAdminOrDev && currentUserId != userId)
                return Forbid();

            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

            if (user == null)
                return NotFound(Result<UserDto>.Failure("No existe el usuario indicado."));

            command.UserId = userId;

            if (!isAdminOrDev)
            {
                command.DealerId = user.DealerId;
                command.Role = user.Role;
                command.DealerType = user.DealerType;
            }

            var result = await _updateUserHandler.HandleAsync(command, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Dev)}")]
        public async Task<IActionResult> Create(
            [FromBody] CreateUserCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _createUserHandler.HandleAsync(command, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        private bool IsAdminOrDev()
        {
            return User.IsInRole(nameof(UserRole.Admin)) || User.IsInRole(nameof(UserRole.Dev));
        }

        private Guid? GetCurrentUserId()
        {
            var userId = User.FindFirstValue("userId")
                ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub");

            return Guid.TryParse(userId, out var parsedUserId)
                ? parsedUserId
                : null;
        }

        private static UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                DealerId = user.DealerId,
                Username = user.Username,
                FirstName = user.FirstName,
                SecondName = user.SecondName,
                LastName = user.LastName,
                SecondLastName = user.SecondLastName,
                Email = user.Email,
                Role = ((int)user.Role).ToString(),
                DealerType = ((int)user.DealerType).ToString()
            };
        }
    }
}
