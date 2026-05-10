using DealerEcommerce.Application.Abstractions;
using DealerEcommerce.Application.Common;
using DealerEcommerce.Application.Users.Commands;
using DealerEcommerce.Application.Users.DTOs;
using DealerEcommerce.Domain.Users;

namespace DealerEcommerce.Application.Users.Handlers
{
    public class CreateUserHandler : ICommandHandler<CreateUserCommand, Result<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public CreateUserHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<Result<UserDto>> HandleAsync(
            CreateUserCommand command,
            CancellationToken cancellationToken = default)
        {
            if (!Enum.IsDefined(command.Role))
                return Result<UserDto>.Failure("El rol del usuario no es válido.");

            if (!Enum.IsDefined(command.DealerType))
                return Result<UserDto>.Failure("El tipo de dealer no es válido.");

            if (string.IsNullOrWhiteSpace(command.Email))
                return Result<UserDto>.Failure("El correo es obligatorio.");

            if (string.IsNullOrWhiteSpace(command.Username))
                return Result<UserDto>.Failure("El usuario es obligatorio.");

            if (string.IsNullOrWhiteSpace(command.Password))
                return Result<UserDto>.Failure("La contraseña es obligatoria.");

            var existingUser = await _userRepository.GetByUsernameOrEmailAsync(
                command.Username,
                cancellationToken);

            if (existingUser != null)
                return Result<UserDto>.Failure("Ya existe un usuario con ese username.");

            existingUser = await _userRepository.GetByUsernameOrEmailAsync(
                command.Email,
                cancellationToken);

            if (existingUser != null)
                return Result<UserDto>.Failure("Ya existe un usuario con ese correo.");

            if (command.Role != UserRole.Dealer && command.DealerType != UserDealerType.NoAplica)
                return Result<UserDto>.Failure("El tipo de dealer solo aplica para usuarios con rol dealer.");

            if (command.Role == UserRole.Dealer && command.DealerType == UserDealerType.NoAplica)
                return Result<UserDto>.Failure("Debe seleccionar un tipo de dealer para usuarios con rol dealer.");

            var passwordHash = _passwordHasher.Hash(command.Password);

            var user = new User(
                null,
                command.Username,
                command.Email,
                passwordHash,
                command.FirstName,
                command.SecondName,
                command.LastName,
                command.SecondLastName,
                command.Role,
                command.Role == UserRole.Dealer
                    ? command.DealerType
                    : UserDealerType.NoAplica);

            await _userRepository.AddAsync(user, cancellationToken);

            var dto = new UserDto
            {
                Id = user.Id,
                DealerId = user.DealerId,
                Username = user.Username,
                Email = user.Email,
                Role = ((int)user.Role).ToString(),
                DealerType = ((int)user.DealerType).ToString()
            };

            return Result<UserDto>.Success(dto, "Usuario creado correctamente.");
        }

    }
}