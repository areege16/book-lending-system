using BookLending.Application.Common.Responses;
using BookLending.Domain.Constants;
using BookLending.Domain.Enums;
using BookLending.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace BookLending.Application.Account.Register
{
    public class RegisterHandler : IRequestHandler<RegisterCommand, ResponseDto<bool>>
    {
        private readonly ILogger<RegisterHandler> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public RegisterHandler(UserManager<ApplicationUser> userManager, ILogger<RegisterHandler> logger)
        {
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<ResponseDto<bool>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var registerRequest = request.RegisterRequestDto;

            _logger.LogInformation("Registration attempt for user: {UserName}", registerRequest.UserName);

            var allowedRoles = new[] { Roles.Reader, Roles.Admin };
            var role = string.IsNullOrEmpty(registerRequest.Role) ? Roles.Reader : registerRequest.Role;

            if (!allowedRoles.Contains(role, StringComparer.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Invalid role specified: {Role}", role);
                return ResponseDto<bool>.Error(ErrorType.BadRequest, "Invalid role. Allowed roles are: Admin, Reader.");
            }

            var user = new ApplicationUser
            {
                UserName = registerRequest.UserName,
                FullName = registerRequest.Name,
                Email = registerRequest.Email,
                PhoneNumber = registerRequest.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, registerRequest.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("User creation failed for {UserName}: {Errors}", registerRequest.UserName, errors);
                return ResponseDto<bool>.Error(ErrorType.UnknownError, $"Account creation failed: {errors}");
            }

            _logger.LogInformation("User created successfully: {UserName}. Assigning to role: {Role}", user.UserName,role);

            var addToRoleResult = await _userManager.AddToRoleAsync(user, role);

            if (!addToRoleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);

                var errors = string.Join(", ", addToRoleResult.Errors.Select(e => e.Description));
                _logger.LogError("Failed to assign role '{Role}' to user {UserName}. Errors: {Errors}. User deleted.", role, user.UserName, errors);

                return ResponseDto<bool>.Error(ErrorType.InternalServerError, $"Failed to assign user role: {errors}");
            }

            _logger.LogInformation("Role '{Role}' assigned successfully to user {UserName}", role, user.UserName);

            return ResponseDto<bool>.Success(true, $"Account created successfully as a {role}.");
        }
    }
}