namespace BookLending.Application.DTOs.Admin.Book
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
