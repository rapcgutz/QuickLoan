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
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, TokenDto>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepo;
        private readonly IUserRepository _userRepo;
        private readonly IAuthService _authService;

        public RefreshTokenCommandHandler(
            IRefreshTokenRepository refreshTokenRepo,
            IUserRepository userRepo,
            IAuthService authService)
        {
            _refreshTokenRepo = refreshTokenRepo;
            _userRepo = userRepo;
            _authService = authService;
        }

        public async Task<TokenDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var refreshToken = await _refreshTokenRepo.GetByTokenAsync(request.RefreshToken);

            if (refreshToken == null || !refreshToken.IsValid())
                throw new UnauthorizedAccessException("Invalid or expired refresh token");

            var user = await _userRepo.GetByIdAsync(refreshToken.UserId);

            if (user == null)
                throw new UnauthorizedAccessException("User not found");

            // Revoke old token
            refreshToken.Revoke();
            await _refreshTokenRepo.UpdateAsync(refreshToken);

            // Generate new tokens
            var newAccessToken = _authService.GenerateAccessToken(user);
            var newRefreshTokenString = _authService.GenerateRefreshToken();
            var newRefreshToken = RefreshToken.Create(user.Id, newRefreshTokenString, DateTime.UtcNow.AddDays(7));

            await _refreshTokenRepo.AddAsync(newRefreshToken);

            return new TokenDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshTokenString,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                Role = user.Role.ToString(),
                User = MapUserToDto(user)
            };
        }

        private UserDto MapUserToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                Mobile = user.Mobile,
                Role = user.Role.ToString(),
                CreatedAt = user.CreatedAt
            };
        }
    }
}
