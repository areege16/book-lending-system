namespace BookLending.Application.DTOs.Admin.Book
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