namespace BookLending.Application.DTOs.Account
{
    public record LoginRequestDto
     (
          string Email,
          string Password
     );
}