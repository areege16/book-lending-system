using BookLending.Application.Abstractions;
using BookLending.Application.Common.Responses;
using BookLending.Application.DTOs.Account;
using BookLending.Application.Setting;
using BookLending.Domain.Enums;
using BookLending.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BookLending.Application.Account.Login
{
    public class LoginHandler : IRequestHandler<LoginCommand, ResponseDto<LoginResponseDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly ILogger<LoginHandler> _logger;
        private readonly JwtSettings _jwtSettings;
        public LoginHandler(UserManager<ApplicationUser> userManager,
                            ITokenService tokenService,
                            IOptions<JwtSettings> jwtSettings,
                            ILogger<LoginHandler> logger)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _logger = logger;
            _jwtSettings = jwtSettings.Value;
        }
        public async Task<ResponseDto<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var loginRequest = request.LoginRequestDto;

            _logger.LogInformation("Login attempt for user {UserName}", loginRequest.UserName);

            ApplicationUser user = await _userManager.FindByNameAsync(loginRequest.UserName);

            if (user == null)
            {
                _logger.LogWarning("Login failed: user not found {UserName}", loginRequest.UserName);
                return ResponseDto<LoginResponseDto>.Error(ErrorType.NotFound, "User not found");
            }

            bool isPasswordCorrect = await _userManager.CheckPasswordAsync(user, loginRequest.Password);
            if (!isPasswordCorrect)
            {
                _logger.LogWarning("Login failed: invalid password for {UserName}", loginRequest.UserName);
                return ResponseDto<LoginResponseDto>.Error(ErrorType.Unauthorized, "Invalid username or password");
            }
            var userRoles = await _userManager.GetRolesAsync(user);
            var accessToken = _tokenService.GenerateAccessToken(user, userRoles);

            _logger.LogInformation("Login successful for user {UserName}", loginRequest.UserName);

            var loginDto = new LoginResponseDto
            (
                user.UserName,
                user.FullName,
                userRoles.FirstOrDefault(),
                accessToken,
                DateTimeOffset.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes)
            );
            return ResponseDto<LoginResponseDto>.Success(loginDto, "Login successful");
        }
    }
}