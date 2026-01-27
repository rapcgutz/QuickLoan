using MediatR;
using QuickLoan.Application.Commands;
using QuickLoan.Application.DTOs;
using QuickLoan.Application.Interfaces;
using QuickLoan.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuickLoan.Application.Handlers.Commands
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, TokenDto>
    {
        private readonly IUserRepository _userRepo;
        private readonly IAuthService _authService;
        private readonly IRefreshTokenRepository _refreshTokenRepo;

        public LoginCommandHandler(
            IUserRepository userRepo,
            IAuthService authService,
            IRefreshTokenRepository refreshTokenRepo)
        {
            _userRepo = userRepo;
            _authService = authService;
            _refreshTokenRepo = refreshTokenRepo;
        }

        public async Task<TokenDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepo.GetByEmailAsync(request.Email);

            if (user == null)
                throw new UnauthorizedAccessException("Invalid email or password");

            if (!_authService.VerifyPassword(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid email or password");

            // Revoke old refresh tokens
            await _refreshTokenRepo.RevokeAllUserTokensAsync(user.Id);

            // Generate new tokens
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
