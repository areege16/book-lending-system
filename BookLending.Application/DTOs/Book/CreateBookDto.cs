namespace BookLending.Application.DTOs.Book
{
    public record CreateBookDto
    (
       string Title,
       string Author,
       string ISBN,
       string? Description,
       string? CoverImage
    );
}