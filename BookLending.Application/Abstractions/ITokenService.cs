using BookLending.Domain.Models;

namespace BookLending.Application.Abstractions
{
    public interface ITokenService
    {
        string GenerateAccessToken(ApplicationUser user, IList<string> roles);
    }
}
