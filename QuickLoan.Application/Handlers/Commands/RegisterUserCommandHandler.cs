using MediatR;
using QuickLoan.Application.Commands;
using QuickLoan.Application.DTOs;
using QuickLoan.Application.Interfaces;
using QuickLoan.Domain.Entities;
using QuickLoan.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuickLoan.Application.Handlers.Commands
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, TokenDto>
    {
        private readonly IUserRepository _userRepo;
        private readonly IAuthService _authService;
        private readonly IRefreshTokenRepository _refreshTokenRepo;

        public RegisterUserCommandHandler(
            IUserRepository userRepo,
            IAuthService authService,
            IRefreshTokenRepository refreshTokenRepo)
        {
            _userRepo = userRepo;
            _authService = authService;
            _refreshTokenRepo = refreshTokenRepo;
        }

        public async Task<TokenDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            // Check if email already exists
            if (await _userRepo.EmailExistsAsync(request.Email))
                throw new InvalidOperationException("Email already registered");

            // Validate age
            if (!User.IsAtLeast18YearsOld(request.DateOfBirth))
                throw new InvalidOperationException("User must be at least 18 years old");

            // Hash password
            var passwordHash = _authService.HashPassword(request.Password);

            // Create user
            var user = User.Create(
                request.Email,
                passwordHash,
                request.FirstName,
                request.LastName,
                request.DateOfBirth,
                request.Mobile,
                UserRole.StandardUser);

            await _userRepo.AddAsync(user);

            // Generate tokens
            var accessToken = _authService.GenerateAccessToken(user);
            var refreshTokenString = _authService.GenerateRefreshToken();
            var refreshToken = RefreshToken.Create(user.Id, refreshTokenString, DateTime.UtcNow.AddDays(7));

            await _refreshTokenRepo.AddAsync(refreshToken);

            return new TokenDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenString,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                Role = user.Role.ToString(),
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    DateOfBirth = user.DateOfBirth,
                    Mobile = user.Mobile,
                    Role = user.Role.ToString(),
                    CreatedAt = user.CreatedAt
                }
            };
        }
    }
}
