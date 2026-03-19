namespace BookLending.Application.DTOs.Book
{
    public record UpdateBookDto
   (
    int Id,
    string Title,
    string Author,
    string ISBN,
    string? Description,
    string? CoverImagePath
   );
}
