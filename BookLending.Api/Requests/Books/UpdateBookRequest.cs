namespace BookLending.Api.Requests.Books
{
    public record UpdateBookRequest
    (
        string Title,
        string Author,
        string ISBN,
        string? Description,
        IFormFile? CoverImage
    );
}