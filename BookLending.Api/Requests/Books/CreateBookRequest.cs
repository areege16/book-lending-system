namespace BookLending.Api.Requests.Books
{
    public record CreateBookRequest
    (
     string Title,
     string Author,
     string ISBN,
     string? Description,
     IFormFile? CoverImage
    );
}