namespace BookLending.Application.DTOs.Account
{
    public record LoginRequestDto
     (
          string UserName,
          string Password
     );
}