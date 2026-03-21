namespace BookLending.Application.DTOs.Account
{
    public record RegisterRequestDto
     (
     string UserName,
     string Name,
     string Password,
     string ConfirmPassword,
     string Email,
     string PhoneNumber,
     string? Role = null
     );
}