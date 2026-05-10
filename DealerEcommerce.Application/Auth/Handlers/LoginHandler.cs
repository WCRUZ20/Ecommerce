using DealerEcommerce.Application.Abstractions;
using DealerEcommerce.Application.Auth.Commands;
using DealerEcommerce.Application.Auth.DTOs;
using DealerEcommerce.Application.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealerEcommerce.Application.Auth.Handlers
{
    public class LoginHandler : ICommandHandler<LoginCommand, Result<LoginResponseDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;

        public LoginHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenService jwtTokenService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<Result<LoginResponseDto>> HandleAsync(
            LoginCommand command,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(command.UsernameOrEmail))
                return Result<LoginResponseDto>.Failure("El usuario o correo es obligatorio.");

            if (string.IsNullOrWhiteSpace(command.Password))
                return Result<LoginResponseDto>.Failure("La contraseña es obligatoria.");

            var user = await _userRepository.GetByUsernameOrEmailAsync(
                command.UsernameOrEmail,
                cancellationToken);

            if (user == null)
                return Result<LoginResponseDto>.Failure("Credenciales incorrectas.");

            var passwordOk = _passwordHasher.Verify(
                command.Password,
                user.PasswordHash);

            if (!passwordOk)
                return Result<LoginResponseDto>.Failure("Credenciales incorrectas.");

            var token = _jwtTokenService.GenerateToken(user);

            var response = new LoginResponseDto
            {
                Token = token,
                UserId = user.Id,
                DealerId = user.DealerId,
                Username = user.Username,
                Email = user.Email,
                Role = ((int)user.Role).ToString(),
                DealerType = ((int)user.DealerType).ToString()
            };

            return Result<LoginResponseDto>.Success(response, "Login correcto.");
        }
    }
}
