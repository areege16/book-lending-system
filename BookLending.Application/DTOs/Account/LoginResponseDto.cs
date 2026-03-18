namespace BookLending.Application.DTOs.Account
{
    public record LoginResponseDto
    (
     string UserName,
     string Name,
     string Role,
     string Token,
     DateTimeOffset AccessTokenExpiresAt
    );
}