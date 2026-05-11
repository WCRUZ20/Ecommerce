using DealerEcommerce.Application.Abstractions;
using DealerEcommerce.Application.Common;
using DealerEcommerce.Application.Users.Commands;
using DealerEcommerce.Application.Users.DTOs;
using DealerEcommerce.Domain.Users;

namespace DealerEcommerce.Application.Users.Handlers
{
    public class UpdateUserHandler : ICommandHandler<UpdateUserCommand, Result<UserDto>>
    {
        private readonly IUserRepository _userRepository;

        public UpdateUserHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<UserDto>> HandleAsync(
            UpdateUserCommand command,
            CancellationToken cancellationToken = default)
        {
            if (command.UserId == Guid.Empty)
                return Result<UserDto>.Failure("El usuario es obligatorio.");

            if (!Enum.IsDefined(command.Role))
                return Result<UserDto>.Failure("El rol del usuario no es válido.");

            if (!Enum.IsDefined(command.DealerType))
                return Result<UserDto>.Failure("El tipo de dealer no es válido.");

            if (string.IsNullOrWhiteSpace(command.Email))
                return Result<UserDto>.Failure("El correo es obligatorio.");

            if (string.IsNullOrWhiteSpace(command.Username))
                return Result<UserDto>.Failure("El usuario es obligatorio.");

            if (string.IsNullOrWhiteSpace(command.FirstName))
                return Result<UserDto>.Failure("El primer nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(command.LastName))
                return Result<UserDto>.Failure("El apellido es obligatorio.");

            if (command.Role != UserRole.Dealer && command.DealerType != UserDealerType.NoAplica)
                return Result<UserDto>.Failure("El tipo de dealer solo aplica para usuarios con rol dealer.");

            if (command.Role == UserRole.Dealer && command.DealerType == UserDealerType.NoAplica)
                return Result<UserDto>.Failure("Debe seleccionar un tipo de dealer para usuarios con rol dealer.");

            if (command.Role != UserRole.Dealer && command.DealerId.HasValue)
                return Result<UserDto>.Failure("Solo los usuarios dealer pueden tener un dealer asociado.");

            var user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);

            if (user == null)
                return Result<UserDto>.Failure("No existe el usuario indicado.");

            var existingUser = await _userRepository.GetByUsernameOrEmailAsync(
                command.Username,
                cancellationToken);

            if (existingUser != null && existingUser.Id != command.UserId)
                return Result<UserDto>.Failure("Ya existe un usuario con ese username.");

            existingUser = await _userRepository.GetByUsernameOrEmailAsync(
                command.Email,
                cancellationToken);

            if (existingUser != null && existingUser.Id != command.UserId)
                return Result<UserDto>.Failure("Ya existe un usuario con ese correo.");

            user.UpdateProfile(
                command.DealerId,
                command.Username,
                command.Email,
                command.FirstName,
                command.SecondName,
                command.LastName,
                command.SecondLastName,
                command.Role,
                command.Role == UserRole.Dealer
                    ? command.DealerType
                    : UserDealerType.NoAplica);

            await _userRepository.UpdateAsync(user, cancellationToken);

            var dto = new UserDto
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

            return Result<UserDto>.Success(dto, "Usuario actualizado correctamente.");
        }
    }
}
